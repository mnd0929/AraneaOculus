using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Interfaces;
using AraneaOculus.Core.Interfaces.Network;
using AraneaOculus.Core.Models.Data;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Network.Shells;
using AraneaOculus.Protocol.Messages.Answers;
using AraneaOculus.Protocol.Messages.Requests;
using AraneaOculus.Protocol.Serialization;

namespace AraneaOculus.Agent.Engine
{
    public class AgentController
    {
        public AgentController(IDataCollector dataCollectionService)
        {
            DataCollectionService = dataCollectionService;
            NetworkClient = new NetworkClientShell(new NetworkPacketSerializer(), SerializationMethod.Json);
        }

        public event EventHandler<string?>? OnNotificationReceived;

        public event EventHandler? Disconnected;

        public event EventHandler? Connected;

        private IDataCollector DataCollectionService { get; set; }

        private INetworkClient NetworkClient { get; set; }

        public async Task Connect(ConnectionAddress networkAddress)
        {
            if (NetworkClient.IsConnected)
                Disconnect();

            NetworkClient.MessageReceived += OnNetworkClientMessageReceived;
            NetworkClient.Connected += Connected;
            NetworkClient.Disconnected += Disconnected;
            await NetworkClient.ConnectAsync(networkAddress);
        }

        public void Disconnect()
        {
            NetworkClient.Disconnect();
            NetworkClient.MessageReceived -= OnNetworkClientMessageReceived;
            NetworkClient.Connected -= Connected;
            NetworkClient.Disconnected -= Disconnected;
        }

        private void OnNetworkClientMessageReceived(object? sender, NetworkMessage e)
        {
            switch (e)
            {
                case NotificationRequestMessage notification:
                    OnNotificationReceived?.Invoke(sender, notification.Text);
                    break;

                case PacketStatisticsDataRequestMessage getPacketStatisticsData:
                    PacketStatisticsData packetStatisticsData = DataCollectionService.GetPacketStatisticsData();
                    var packetStatisticsDataAnswerMessage = new PacketStatisticsDataAnswerMessage(getPacketStatisticsData)
                    {
                        Data = packetStatisticsData
                    };
                    NetworkClient.SendAsync(packetStatisticsDataAnswerMessage);
                    break;

                case UniqueIdentifierRequestMessage uniqueIdentifierRequestMessage:
                    string id = DataCollectionService.GetUniqueIdentifier();
                    var uniqueIdentifierAnswer = new UniqueIdentifierAnswerMessage(uniqueIdentifierRequestMessage)
                    {
                        Identifier = id
                    };
                    NetworkClient.SendAsync(uniqueIdentifierAnswer);
                    break;
            }
        }
    }
}
