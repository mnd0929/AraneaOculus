using AraneaOculus.Core.Enums;

namespace AraneaOculus.Core.Models.Network
{
    public abstract class NetworkMessage
    {
        public NetworkMessage()
        {
            CreationTime = DateTime.UtcNow;
        }

        public Credentials? SenderCredentials { get; set; }

        public DateTime? CreationTime { get; set; }

        public string? Id { get; set; }

        public abstract NetworkMessageType MessageType { get; }

        public abstract NetworkCommunicationType CommunicationType { get; }
    }
}