using System.Net;

namespace AraneaOculus.Core.Models.Network
{
    public class ConnectionAddress
    {
        public IPAddress Host { get; set; }

        public int Port { get; set; }

        public ConnectionAddress(int port)
        {
            Host = IPAddress.Any;
            Port = port;
        }

        public ConnectionAddress(string address, int port)
        {
            Host = IPAddress.Parse(address);
            Port = port;
        }

        public ConnectionAddress(IPAddress address, int port)
        {
            Host = address;
            Port = port;
        }

        public ConnectionAddress(string address)
        {
            Host = IPAddress.Parse(address);
            Port = 0;
        }

        public ConnectionAddress(IPAddress address)
        {
            Host = address;
            Port = 0;
        }
    }
}
