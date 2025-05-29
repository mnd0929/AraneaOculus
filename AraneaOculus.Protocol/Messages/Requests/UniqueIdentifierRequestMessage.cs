using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Protocol.Messages.Requests
{
    public class UniqueIdentifierRequestMessage : Request
    {
        public override NetworkMessageType MessageType => NetworkMessageType.GetUniqueIdentifier;
    }
}
