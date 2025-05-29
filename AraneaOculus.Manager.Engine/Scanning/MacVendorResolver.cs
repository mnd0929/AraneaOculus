using System.Text.RegularExpressions;

namespace AraneaOculus.Manager.Engine.Scanning
{
    public class MacVendorResolver
    {
        private const string OuiDatabaseUrl = "https://standards-oui.ieee.org/oui/oui.txt";

        private static Dictionary<string, string> OuiDatabase = new();

        public static async Task InitializeAsync()
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AraneaOculus/1.0)");
            var ouiText = await httpClient.GetStringAsync(OuiDatabaseUrl);

            var lines = ouiText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                               .Where(line => line.Contains("(hex)"));

            foreach (var line in lines)
            {
                int hexIndex = line.IndexOf("(hex)");
                if (hexIndex == -1) continue;

                string ouiPart = line.Substring(0, hexIndex).Trim();
                string vendor = line.Substring(hexIndex + "(hex)".Length).Trim();

                ouiPart = ouiPart.Replace("-", "");
                if (ouiPart.Length < 6) continue;
                string oui = ouiPart.Substring(0, 6).ToUpper();

                OuiDatabase[oui] = vendor;
            }
        }

        public static string GetVendor(string mac)
        {
            if (mac == null)
                return "Unknown";

            var cleanMac = Regex.Replace(mac, "[^A-Fa-f0-9]", "");
            if (cleanMac.Length < 6) return "Unknown";

            var oui = cleanMac.Substring(0, 6).ToUpper();
            return OuiDatabase.TryGetValue(oui, out var vendor) ? vendor : "Unknown";
        }
    }
}
