using AraneaOculus.Core.Models.Network;
using System.Net;

namespace AraneaOculus.Core.Models.Identification
{
    public class DeviceSigns
    {
        public DeviceSigns(NetworkDevice networkDevice)
        {
            UniqueIdentifier = networkDevice.DeviceId;
            MacAddress = networkDevice.MacAddress;
            Address = networkDevice.ConnectionAddress.Host;
            Name = networkDevice.Info.Name;
        }

        public string? UniqueIdentifier { get; set; }

        public string? MacAddress { get; set; }

        public IPAddress? Address { get; set; }

        public string? Name { get; set; }
    }
}
