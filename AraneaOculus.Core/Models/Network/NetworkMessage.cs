using AraneaOculus.Core.Enums;

namespace AraneaOculus.Core.Models.Network
{
    public abstract class NetworkMessage
    {
        public NetworkMessage()
        {
            Id = Guid.NewGuid().ToString();
            CreationTime = DateTime.UtcNow;
        }

        public Credentials? SenderCredentials { get; set; }

        public DateTime? CreationTime { get; set; }

        public string? Id { get; internal set; }

        public abstract NetworkMessageType MessageType { get; }

        public abstract NetworkCommunicationType CommunicationType { get; }
    }
}