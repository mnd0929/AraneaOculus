using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Core.Interfaces.Network
{
    public interface INetworkPacketSerializer
    {
        byte[] Serialize(NetworkMessage networkMessage, SerializationMethod serializationMethod);

        Task<NetworkMessage> Deserialize(byte[] data);
    }
}
