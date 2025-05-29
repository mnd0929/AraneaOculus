using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Core.Interfaces.Network
{
    public interface IMessageSerializer
    {
        byte[] Serialize(NetworkMessage networkMessage);
        
        /// <param name="data">2 байта протокола + сериализованные данные</param>
        Task<NetworkMessage> Deserialize(byte[] data, CancellationToken cancellationToken = default);
    }
}
