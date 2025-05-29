using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Protocol.Messages.Requests
{
    public class AuthorizationRequestMessage : Request
    {
        public override NetworkMessageType MessageType => NetworkMessageType.Authorization;

        public Credentials? Key { get; set; }
    }
}
