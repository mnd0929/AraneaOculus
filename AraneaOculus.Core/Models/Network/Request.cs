using AraneaOculus.Core.Enums;

namespace AraneaOculus.Core.Models.Network
{
    public abstract class Request : NetworkMessage
    {
        public override NetworkCommunicationType CommunicationType => NetworkCommunicationType.Request;

        public bool IsAnswerRequired { get; set; }
    }
}
