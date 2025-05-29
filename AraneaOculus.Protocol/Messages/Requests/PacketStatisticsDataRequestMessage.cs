using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Protocol.Messages.Requests
{
    public class PacketStatisticsDataRequestMessage : Request
    {
        public override NetworkMessageType MessageType => NetworkMessageType.GetPacketStatisticsData;
    }
}
