using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Protocol.Messages.Requests
{
    public class NotificationRequestMessage : Request
    {
        public override NetworkMessageType MessageType => NetworkMessageType.Notification;

        public string? Text { get; set; }
    }
}
