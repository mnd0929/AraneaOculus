using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Data;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Protocol.Messages.Answers
{
    public class PacketStatisticsDataAnswerMessage : Answer
    {
        public PacketStatisticsDataAnswerMessage() { }

        public PacketStatisticsDataAnswerMessage(Request request) : base(request) { }

        public override NetworkMessageType MessageType => NetworkMessageType.GetPacketStatisticsData;

        public PacketStatisticsData? Data { get; set; }
    }
}
