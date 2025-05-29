using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
#if WINDOWS
using System.Management;
#endif

namespace AraneaOculus.Manager.Engine.Scanning
{
    public static class NetworkDeviceNameResolver
    {
        /// <summary>
        /// Получает имя устройства по IP или MAC адресу, используя последовательность методов: DNS, LLMNR, UPnP и WMI.
        /// Возвращает null, если имя не удалось определить.
        /// </summary>
        public static async Task<string?> GetDeviceNameAsync(string ipAddress, string? macAddress = null)
        {
            string? deviceName;

            // 1. Попытка разрешить через DNS
            deviceName = await ResolveWithDnsAsync(ipAddress);
            if (!string.IsNullOrWhiteSpace(deviceName))
                return deviceName;

            // 2. Попытка разрешить через LLMNR
            deviceName = await ResolveWithLLMNRAsync(ipAddress);
            if (!string.IsNullOrWhiteSpace(deviceName))
                return deviceName;

            // 3. Попытка разрешить через UPnP
            deviceName = await ResolveWithUPnPAsync(ipAddress);
            if (!string.IsNullOrWhiteSpace(deviceName))
                return deviceName;

#if WINDOWS
            // 4. Попытка разрешить через WMI (только для Windows)
            deviceName = await ResolveWithWmiAsync(ipAddress);
            if (!string.IsNullOrWhiteSpace(deviceName))
                return deviceName;
#endif

            return null;
        }

        private static async Task<string?> ResolveWithDnsAsync(string ipAddress)
        {
            try
            {
                IPHostEntry entry = await Dns.GetHostEntryAsync(ipAddress);
                if (!string.IsNullOrWhiteSpace(entry.HostName))
                    return entry.HostName.TrimEnd('.');
            }
            catch (Exception)
            {
                // Игнорируем ошибки DNS
            }
            return null;
        }

        private static async Task<string?> ResolveWithLLMNRAsync(string ipAddress)
        {
            // Реализуем обратный запрос LLMNR для PTR записи
            // Формируем доменное имя вида "X.X.X.X.in-addr.arpa"
            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return null;
            string reverseName = $"{parts[3]}.{parts[2]}.{parts[1]}.{parts[0]}.in-addr.arpa";

            try
            {
                // Формирование DNS запроса в бинарном виде
                byte[] request = CreateDnsQueryPacket(reverseName, qType: 12); // 12 = PTR
                using (UdpClient udpClient = new UdpClient())
                {
                    udpClient.Client.ReceiveTimeout = 2000;
                    // LLMNR multicast IPv4 адрес и порт
                    IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("224.0.0.252"), 5355);
                    await udpClient.SendAsync(request, request.Length, multicastEP);

                    var responseTask = udpClient.ReceiveAsync();
                    if (await Task.WhenAny(responseTask, Task.Delay(2000)) == responseTask)
                    {
                        UdpReceiveResult result = responseTask.Result;
                        string? name = ParseDnsResponse(result.Buffer);
                        if (!string.IsNullOrWhiteSpace(name))
                            return name;
                    }
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки LLMNR
            }
            return null;
        }

        private static byte[] CreateDnsQueryPacket(string domainName, ushort qType)
        {
            // Стандартный DNS-запрос без дополнительных секций
            // ID = 0x1234 (любое значение)
            // Флаги = 0x0000, QDCOUNT = 1, ANCOUNT = 0, NSCOUNT = 0, ARCOUNT = 0.
            byte[] id = { 0x12, 0x34 };
            byte[] flags = { 0x00, 0x00 };
            byte[] qdcount = { 0x00, 0x01 };
            byte[] ancount = { 0x00, 0x00 };
            byte[] nscount = { 0x00, 0x00 };
            byte[] arcount = { 0x00, 0x00 };

            var header = id.Concat(flags).Concat(qdcount).Concat(ancount).Concat(nscount).Concat(arcount).ToArray();

            // Формирование вопроса
            // Имя представлено сериями меток, каждая начинается с длины
            var sb = new StringBuilder();
            foreach (var label in domainName.Split('.'))
            {
                sb.Append((char)label.Length);
                sb.Append(label);
            }
            sb.Append((char)0); // завершающий 0

            byte[] qname = Encoding.ASCII.GetBytes(sb.ToString());
            byte[] qtype = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)qType));
            byte[] qclass = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)1)); // IN

            return header.Concat(qname).Concat(qtype).Concat(qclass).ToArray();
        }

        private static string? ParseDnsResponse(byte[] response)
        {
            // Минимальный парсер для ответа DNS: пропускаем заголовок и вопрос, затем читаем ответную запись PTR.
            try
            {
                // id(2) flags(2) qdcount(2) ancount(2)
                if (response.Length < 12)
                    return null;
                ushort qdCount = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 4));
                ushort anCount = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, 6));
                if (anCount < 1)
                    return null;
                int offset = 12;
                // Пропускаем вопросы
                for (int i = 0; i < qdCount; i++)
                {
                    // Пропускаем QNAME
                    while (offset < response.Length && response[offset] != 0)
                    {
                        offset += response[offset] + 1;
                    }
                    offset += 1; // нулевой байт
                    offset += 4; // QTYPE (2) + QCLASS (2)
                }
                // Читаем первый ответ
                // Пропускаем имя (может быть указатель)
                if ((response[offset] & 0xC0) == 0xC0)
                {
                    offset += 2;
                }
                else
                {
                    while (offset < response.Length && response[offset] != 0)
                    {
                        offset += response[offset] + 1;
                    }
                    offset += 1;
                }
                if (offset + 10 > response.Length)
                    return null;
                // TYPE(2), CLASS(2), TTL(4), RDLENGTH(2)
                ushort type = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, offset));
                offset += 2;
                offset += 2; // класс
                offset += 4; // TTL
                ushort rdLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(response, offset));
                offset += 2;

                if (type != 12 || rdLength <= 0)
                    return null;

                // Читаем RDATA - DNS-имя
                string result = "";
                int end = offset + rdLength;
                while (offset < end && response[offset] != 0)
                {
                    int len = response[offset];
                    offset++;
                    if (offset + len > end)
                        break;
                    result += Encoding.ASCII.GetString(response, offset, len) + ".";
                    offset += len;
                }
                return result.TrimEnd('.');
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string?> ResolveWithUPnPAsync(string ipAddress)
        {
            // Ищем устройство UPnP по SSDP посредством отправки M-SEARCH и фильтруем ответы по IP
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    udpClient.Client.ReceiveTimeout = 3000;
                    IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
                    udpClient.Client.Bind(localEp);

                    IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

                    string mSearch = "M-SEARCH * HTTP/1.1\r\n" +
                                     "HOST: 239.255.255.250:1900\r\n" +
                                     "MAN: \"ssdp:discover\"\r\n" +
                                     "MX: 1\r\n" +
                                     "ST: ssdp:all\r\n" +
                                     "\r\n";

                    byte[] requestBytes = Encoding.ASCII.GetBytes(mSearch);
                    await udpClient.SendAsync(requestBytes, requestBytes.Length, multicastEP);

                    DateTime start = DateTime.UtcNow;
                    while ((DateTime.UtcNow - start).TotalMilliseconds < 3000)
                    {
                        if (udpClient.Available > 0)
                        {
                            var result = await udpClient.ReceiveAsync();
                            // Если IP отправителя совпадает с искомым, анализируем ответ
                            if (result.RemoteEndPoint.Address.ToString() == ipAddress)
                            {
                                string responseText = Encoding.ASCII.GetString(result.Buffer);
                                // Ищем заголовок LOCATION
                                string location = responseText.Split("\r\n")
                                    .FirstOrDefault(x => x.StartsWith("LOCATION:", StringComparison.OrdinalIgnoreCase))?.Split(':', 2)[1].Trim() ?? "";
                                if (!string.IsNullOrEmpty(location))
                                {
                                    string friendlyName = await GetUpnpFriendlyNameAsync(location);
                                    if (!string.IsNullOrWhiteSpace(friendlyName))
                                        return friendlyName;
                                }
                            }
                        }
                        else
                        {
                            await Task.Delay(100);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки UPnP
            }
            return null;
        }

        private static async Task<string?> GetUpnpFriendlyNameAsync(string location)
        {
            try
            {
                using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                var response = await httpClient.GetAsync(location);
                if (!response.IsSuccessStatusCode)
                    return null;
                string xmlContent = await response.Content.ReadAsStringAsync();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);
                XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
                ns.AddNamespace("upnp", "urn:schemas-upnp-org:device-1-0");

                var node = xmlDoc.SelectSingleNode("//upnp:friendlyName", ns);
                if (node != null)
                    return node.InnerText;
            }
            catch (Exception)
            {
                // Игнорируем ошибки разбора UPnP
            }
            return null;
        }

#if WINDOWS
        private static async Task<string?> ResolveWithWmiAsync(string ipAddress)
        {
            // Используем WMI для получения имени удалённого компьютера
            // Требуется, чтобы удалённый WMI был включён и был настроен DCOM
            return await Task.Run(() =>
            {
                try
                {
                    string path = $"\\\\{ipAddress}\\root\\cimv2";
                    var scope = new ManagementScope(path);
                    scope.Connect();
                    ObjectQuery query = new ObjectQuery("SELECT Caption FROM Win32_ComputerSystem");
                    using ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string? caption = mo["Caption"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(caption))
                            return caption;
                    }
                }
                catch (Exception)
                {
                    // Игнорируем любые ошибки WMI
                }
                return (string?)null;
            });
        }
#endif
    }
}