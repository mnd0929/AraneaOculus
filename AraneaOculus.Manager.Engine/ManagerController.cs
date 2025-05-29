using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Interfaces.Network;
using AraneaOculus.Core.Models;
using AraneaOculus.Core.Models.Data;
using AraneaOculus.Core.Models.Data.Statistics;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Core.Utilities;
using AraneaOculus.Manager.Engine.Configuration;
using AraneaOculus.Manager.Engine.Scanning;
using AraneaOculus.Network.Shells;
using AraneaOculus.Protocol.Messages.Answers;
using AraneaOculus.Protocol.Messages.Requests;
using AraneaOculus.Protocol.Serialization;
using System.Collections.ObjectModel;

namespace AraneaOculus.Manager.Engine
{
    public class ManagerController
    {
        public ManagerController(ManagerConfiguration configuration) 
        {
            Configuration = configuration;
            Registry = new DeviceRegistry();
            IdStorage = new IdentifierStorage();
            ConnectionsHistory = new ObservableDictionary<Guid, ObservableCollection<ConnectionsHistoryEntry>>();
            PacketHistory = new ObservableDictionary<Guid, ObservableCollection<PacketHistoryEntry>>();
            Scanner = new NetworkScanner(configuration.HostAddress!.Host);
            Server = new NetworkServerShell(new NetworkPacketSerializer(), SerializationMethod.Json);
        }

        public bool IsWorks => Server.IsWorks;

        public ObservableCollection<NetworkDevice>? Devices => Registry.Devices.Values as ObservableCollection<NetworkDevice>;

        public ObservableDictionary<Guid, ObservableCollection<ConnectionsHistoryEntry>>? ConnectionsHistory;

        public ObservableDictionary<Guid, ObservableCollection<PacketHistoryEntry>>? PacketHistory;

        public event EventHandler<ManagerStatus>? OnStatusChanged;

        private SynchronizationContext? Context;

        private ManagerConfiguration Configuration;

        private DeviceRegistry Registry;

        private IdentifierStorage IdStorage;

        private NetworkScanner Scanner;

        private INetworkServer Server;

        private CancellationTokenSource? TokenSource;

        public void SetSynchronizationContext(SynchronizationContext context)
        {
            Context = context;
            Scanner.SetSynchronizationContext(context);
        }

        public async Task Start()
        {
            TokenSource = new CancellationTokenSource();

            SetStatus(ManagerStatus.UpdatingVendorData);
            await MacVendorResolver.InitializeAsync();

            SetStatus(ManagerStatus.LaunchingNetworkListening);
            Server.ClientConnected += ClientConnected;
            Server.ClientDisconnected += ClientDisconnected;
            Server.MessageReceived += MessageReceived;
            _ = Server.StartAsync(Configuration.HostAddress!);

            SetStatus(ManagerStatus.LaunchingNetworkScanning);
            Scanner.OnlineDevices.CollectionChanged += OnlineDevicesCollectionChanged;
            Scanner.Start();

            SetStatus(ManagerStatus.NetworkMonitoring);
        }

        public void Stop()
        {
            TokenSource?.Cancel();

            SetStatus(ManagerStatus.TerminatingNetworkListening);
            Server.ClientConnected -= ClientConnected;
            Server.ClientDisconnected -= ClientDisconnected;
            Server.MessageReceived -= MessageReceived;
            Server.Stop();

            SetStatus(ManagerStatus.TerminatingNetworkScanning);
            Scanner.OnlineDevices.CollectionChanged -= OnlineDevicesCollectionChanged;
            Scanner.Stop();

            SetStatus(ManagerStatus.Inaction);
        }

        private NetworkDevice CreateDevice(Guid deviceId, ConnectionAddress connectionAddress, DetectionType detectionType)
        {
            NetworkDevice networkDevice = new NetworkDevice
            {
                CurrentTrustLevel = TrustLevel.NoTrust,
                ConnectionAddress = connectionAddress,
                DetectionType = detectionType
            };

            if (deviceId != Guid.Empty)
                networkDevice.ConnectionId = deviceId;

            return networkDevice;
        }

        private async Task AddDevice(NetworkDevice device)
        {
            Context?.Post(_ => Registry.Add(device), null);
            
            await Task.Run(async () =>
            {
                string macAddress = (await ArpLookup.Arp.LookupAsync(device.ConnectionAddress.Host))?.ToString()!;

                device.MacAddress = macAddress;
                device.Info.Vendor = MacVendorResolver.GetVendor(device.MacAddress);
                device.Info.Name = (await NetworkDeviceNameResolver.GetDeviceNameAsync(device.ConnectionAddress.Host.ToString()))!;
                device.SignBasedIdentifier = IdStorage.GetIdentifier(device);

                if (macAddress != null)
                    Registry.BeginTrusting(device.ConnectionId);
            });
        }

        private void RemoveDevice(Guid guid)
        {
            NetworkDevice networkDevice = Registry.GetDeviceById(guid);

            Context?.Post(_ => Registry.Remove(guid), null);
        }

        private void SetStatus(ManagerStatus managerStatus)
        {
            OnStatusChanged?.Invoke(this, managerStatus);
        }

        private void RegisterConnection(NetworkDevice networkDevice)
        {
            Context?.Post(_ => 
            {
                ConnectionsHistoryEntry entry = new ConnectionsHistoryEntry 
                {
                    ConnectedAt = DateTime.UtcNow,
                };

                Guid id = IdStorage.GetIdentifier(networkDevice).Token;

                if (!ConnectionsHistory!.ContainsKey(id))
                    ConnectionsHistory.Add(id, new ObservableCollection<ConnectionsHistoryEntry> { entry });
                else ConnectionsHistory[id].Add(entry);
            }, null);
        }

        private void RegisterDisconnection(NetworkDevice networkDevice)
        {
            Context?.Post(_ =>
            {
                ConnectionsHistoryEntry entry = new ConnectionsHistoryEntry
                {
                    DisconnectedAt = DateTime.UtcNow,
                };

                Guid id = IdStorage.GetIdentifier(networkDevice).Token;

                if (!ConnectionsHistory!.ContainsKey(id))
                    ConnectionsHistory.Add(id, new ObservableCollection<ConnectionsHistoryEntry> { entry });
                else ConnectionsHistory[id].Add(entry);
            }, null);
        }

        private void RegisterPacketCount(NetworkDevice networkDevice, PacketStatisticsDataAnswerMessage packetStatisticsDataAnswerMessage)
        {
            Context?.Post(_ =>
            {
                PacketHistoryEntry packetHistoryEntry = new PacketHistoryEntry
                {
                    Received = packetStatisticsDataAnswerMessage?.Data?.Received ?? 0,
                    Sent = packetStatisticsDataAnswerMessage?.Data?.Sent ?? 0,
                    Timestamp = packetStatisticsDataAnswerMessage?.CreationTime ?? DateTime.Now,
                };

                Guid id = IdStorage.GetIdentifier(networkDevice).Token;

                if (!PacketHistory!.ContainsKey(id))
                    PacketHistory.Add(id, new ObservableCollection<PacketHistoryEntry> { packetHistoryEntry });
                else PacketHistory[id].Add(packetHistoryEntry);
            }, null);
        }

        private async Task StartAgentInformationUpdateLoop(NetworkDevice networkDevice)
        {
            while (!TokenSource!.IsCancellationRequested)
            {
                var packetStatisticsDataAnswer =
                    (PacketStatisticsDataAnswerMessage)await Server.SendRequestAsync(networkDevice.ConnectionId, new PacketStatisticsDataRequestMessage(), cancellationToken: TokenSource.Token);

                RegisterPacketCount(networkDevice, packetStatisticsDataAnswer);

                networkDevice.Info.PacketStatistics = packetStatisticsDataAnswer.Data!;

                await Task.Delay(1000);
            }
        }

        private void MessageReceived(object? sender, DataReceivedEventArgs e)
        {
            if (e.Message?.CommunicationType == NetworkCommunicationType.Answer)
                return;


        }

        private async void ClientConnected(object? sender, Guid e)
        {
            ConnectionAddress connectionAddress = Server.GetClientConnectionAddress(e);

            NetworkDevice networkDevice = CreateDevice(e, connectionAddress, DetectionType.Agent);
            await AddDevice(networkDevice);

            var idAnswer = (UniqueIdentifierAnswerMessage)await Server.SendRequestAsync(e, new UniqueIdentifierRequestMessage());
            if (idAnswer.Identifier != null)
            {
                networkDevice.DeviceId = idAnswer.Identifier;

                Registry.BeginTrusting(e, true);

                _ = StartAgentInformationUpdateLoop(networkDevice);
            }

            RegisterConnection(networkDevice);
        }

        private void ClientDisconnected(object? sender, Guid e)
        {
            NetworkDevice device = Registry.GetDeviceById(e);
            RegisterDisconnection(device);

            RemoveDevice(e);
        }

        private async void OnlineDevicesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var address in e.NewItems)
                {
                    var device = CreateDevice(Guid.Empty, new ConnectionAddress((System.Net.IPAddress)address), DetectionType.NetworkScanning);
                    await AddDevice(device);

                    RegisterConnection(device);
                }
            }
            if (e.OldItems != null)
            {
                foreach (var address in e.OldItems)
                {
                    NetworkDevice device =
                        Registry.GetDevices().Find(device => device.ConnectionAddress.Host == (System.Net.IPAddress)address)!;
                    RegisterDisconnection(device);
                    RemoveDevice(device.ConnectionId);
                }
            }
        }
    }
}
