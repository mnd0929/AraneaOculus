using AraneaOculus.Core.Interfaces;
using AraneaOculus.Core.Models.Data;
using System.Net.NetworkInformation;

namespace AraneaOculus.Agent.Services.Platforms.Windows
{
    public class WindowsDataCollector : IDataCollector
    {
        public PacketStatisticsData GetPacketStatisticsData()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                       n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .ToList();

            long received = interfaces.Sum(i => i.GetIPStatistics().BytesReceived);
            long sent = interfaces.Sum(i => i.GetIPStatistics().BytesSent);

            return new PacketStatisticsData 
            {
                Received = received,
                Sent = sent
            };
        }

        public string GetUniqueIdentifier()
        {
            return Utilities.DeviceIdGenerator.GetDeviceId();
        }
    }
}
