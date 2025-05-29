using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Core.Utilities;
using System.ComponentModel;

namespace AraneaOculus.Manager.Engine
{
    public class DeviceRegistry
    {
        public readonly ObservableDictionary<Guid, NetworkDevice> Devices;

        public DeviceRegistry() => Devices = new();

        public void Add(NetworkDevice networkDevice)
        {
            if (!Devices.ContainsKey(networkDevice.ConnectionId))
            {
                Devices.Add(networkDevice.ConnectionId, networkDevice);
                networkDevice.PropertyChanged += OnDevicePropertyChanged;
            }
        }

        public void Remove(Guid id)
        {
            if (Devices.ContainsKey(id))
                Devices.Remove(id);
        }

        public void BeginTrusting(Guid id, bool isFullyTrusted = false)
        {
            if (!ValidateId(id))
                return;

            Devices[id].CurrentTrustLevel = isFullyTrusted ? TrustLevel.Trust : TrustLevel.PotentialDanger;
        }

        public void TerminateTrusting(Guid id)
        {
            if (!ValidateId(id))
                return;

            Devices[id].CurrentTrustLevel = TrustLevel.NoTrust;
        }

        public List<Guid> GetIDs() => [.. Devices.Keys];

        public List<NetworkDevice> GetDevices() => [.. Devices.Values];

        public NetworkDevice GetDeviceById(Guid id)
        {
            if (!ValidateId(id))
                return null!;

            return Devices[id];
        }

        private bool ValidateId(Guid id)
        {
            return Devices.ContainsKey(id);
        }

        private void OnDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is NetworkDevice device && e.PropertyName == nameof(NetworkDevice.ConnectionId))
            {
                var oldEntry = Devices.FirstOrDefault(kvp => kvp.Value == device);
                if (!oldEntry.Equals(default(KeyValuePair<Guid, NetworkDevice>)) && oldEntry.Key != device.ConnectionId)
                {
                    Devices.Remove(oldEntry.Key);
                    if (!Devices.ContainsKey(device.ConnectionId))
                        Devices.Add(device.ConnectionId, device);
                }
            }
        }
    }
}
