using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AraneaOculus.Core.Utilities
{
    public class NetworkUtils
    {
        public static IEnumerator<UnicastIPAddressInformation> GetLocalUnicastAddresses()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                            n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            foreach (var networkInterface in networkInterfaces)
            {
                foreach (var ipInfo in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        yield return ipInfo;
                }
            }
        }

        public static IEnumerable<IPAddress> GenerateIPAddressesInRange(IPAddress networkAddress, IPAddress broadcastAddress)
        {
            uint start = BitConverter.ToUInt32(networkAddress.GetAddressBytes().Reverse().ToArray(), 0);
            uint end = BitConverter.ToUInt32(broadcastAddress.GetAddressBytes().Reverse().ToArray(), 0);

            for (uint i = start; i <= end; i++)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                yield return new IPAddress(new[] { bytes[3], bytes[2], bytes[1], bytes[0] });
            }
        }

        public static IPAddress CalculateStopAddress(IPAddress address)
        {
            byte[] ipBytes = address.GetAddressBytes();
            return new IPAddress(new byte[] { ipBytes[0], ipBytes[1], 255, 255 });
        }

        public static IPAddress CalculateStartAddress(IPAddress address)
        {
            byte[] ipBytes = address.GetAddressBytes();
            return new IPAddress(new byte[] { ipBytes[0], ipBytes[1], 0, 0 });
        }

        public static async Task<bool> PingIPAsync(IPAddress ip, int timeout)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var pingTask = ping.SendPingAsync(ip);
                    var delay = Task.Delay(timeout);
                    var completed = await Task.WhenAny(pingTask, delay);
                    if (completed == delay)
                        return false;
                    return pingTask.Result.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> CheckPortAsync(string ip, int port, int timeout = 300)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var task = client.ConnectAsync(ip, port);
                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        await task;
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PORTS@{ip}] {port} EXCEPTION: {ex}");
                return false;
            }
        }
    }
}
