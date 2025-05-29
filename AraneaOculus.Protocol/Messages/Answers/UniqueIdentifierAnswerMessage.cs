using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Protocol.Messages.Answers
{
    public class UniqueIdentifierAnswerMessage : Answer
    {
        public UniqueIdentifierAnswerMessage() { }

        public UniqueIdentifierAnswerMessage(Request request) : base(request) { }

        public override NetworkMessageType MessageType => NetworkMessageType.GetUniqueIdentifier;

        public string? Identifier { get; set; }
    }
}
