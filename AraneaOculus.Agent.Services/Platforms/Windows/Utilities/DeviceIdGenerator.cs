using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace AraneaOculus.Agent.Services.Platforms.Windows.Utilities
{
    public class DeviceIdGenerator
    {
        private static string? _deviceId;

        public static string GetDeviceId()
        {
            if (!string.IsNullOrEmpty(_deviceId))
                return _deviceId;

            var sb = new StringBuilder();

            AppendComponent(sb, GetProcessorId());
            AppendComponent(sb, GetMotherboardId());
            AppendComponent(sb, GetDiskId());
            AppendComponent(sb, GetBiosId());
            AppendComponent(sb, GetVideoControllerId());

            _deviceId = GenerateHash(sb.ToString());
            return _deviceId;
        }

        private static void AppendComponent(StringBuilder sb, string component)
        {
            if (!string.IsNullOrEmpty(component))
                sb.Append(component + "|");
        }

        private static string GenerateHash(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static string GetWmiProperty(string wmiClass, string property)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {wmiClass}");
                foreach (ManagementObject obj in searcher.Get())
                    return obj[property]?.ToString()!.Trim()!;
            }
            catch { }
            return string.Empty;
        }

        private static string GetProcessorId()
            => GetWmiProperty("Win32_Processor", "ProcessorId");

        private static string GetMotherboardId()
            => GetWmiProperty("Win32_BaseBoard", "SerialNumber");

        private static string GetDiskId()
            => GetWmiProperty("Win32_DiskDrive", "SerialNumber");

        private static string GetBiosId()
            => GetWmiProperty("Win32_BIOS", "SerialNumber");

        private static string GetVideoControllerId()
            => GetWmiProperty("Win32_VideoController", "DeviceID");
    }
}
