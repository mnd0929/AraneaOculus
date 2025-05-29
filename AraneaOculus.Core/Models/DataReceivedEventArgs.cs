using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Core.Models
{
    public class DataReceivedEventArgs
    {
        public Guid? ClientId { get; set; }

        public NetworkMessage? Message { get; set; }
    }
}
