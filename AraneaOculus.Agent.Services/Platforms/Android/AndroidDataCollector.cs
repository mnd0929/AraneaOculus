using Android.Net;
using Android.Provider;
using AraneaOculus.Core.Interfaces;
using AraneaOculus.Core.Models.Data;

namespace AraneaOculus.Agent.Services.Platforms.Android
{
    public class AndroidDataCollector : IDataCollector
    {
        public PacketStatisticsData GetPacketStatisticsData()
        {
            return new PacketStatisticsData
            {
                Received = TrafficStats.TotalRxBytes,
                Sent = TrafficStats.TotalTxBytes
            };
        }

        public string GetUniqueIdentifier()
        {
            return Settings.Secure.GetString(Platform.CurrentActivity!.ContentResolver, Settings.Secure.AndroidId)!;
        }
    }
}
