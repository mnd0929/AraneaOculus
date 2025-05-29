using AraneaOculus.Core.Models.Data;

namespace AraneaOculus.Core.Interfaces
{
    public interface IDataCollector
    {
        PacketStatisticsData GetPacketStatisticsData();

        string GetUniqueIdentifier();
    }
}
