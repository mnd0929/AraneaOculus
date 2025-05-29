using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Interfaces.Network;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Core.Utilities;
using AraneaOculus.Protocol.Serialization.Serializers;

namespace AraneaOculus.Protocol.Serialization
{
    public class NetworkPacketSerializer : INetworkPacketSerializer
    {
        public byte[] Serialize(NetworkMessage networkMessage, SerializationMethod serializationMethod)
        {
            byte[] data = CreateSerializer(serializationMethod).Serialize(networkMessage);

            byte[] protocolVersionSegment = BitConverter.GetBytes(Meta.VersionNumber);
            EndiannesGuarantor.GuaranteeLittleEndian(protocolVersionSegment);

            byte[] method = [(byte)serializationMethod];

            return
            [   
                .. protocolVersionSegment,
                .. method,
                .. data
            ];
        }

        public async Task<NetworkMessage> Deserialize(byte[] data)
        {
            const int headerSize = 2 + 1;

            if (data.Length < headerSize)
                throw new InvalidDataException("Not enough data to deserialize the header (MessageSerializer)");

            // Protocol Version (2 байта)
            byte[] versionBuffer = new byte[2];
            Buffer.BlockCopy(data, 0, versionBuffer, 0, 2);
            EndiannesGuarantor.GuaranteeLittleEndian(versionBuffer);
            short version = BitConverter.ToInt16(versionBuffer, 0);

            if (version != Meta.VersionNumber)
                throw new InvalidDataException($"Incompatible AraneaContract protocol version: v{version}. Expected protocol version: {Meta.VersionName} (v{Meta.VersionNumber})");

            // Serialization Method (1 байт)
            SerializationMethod serializationMethod = (SerializationMethod)data[2];

            // Чтение данных сообщения
            int dataLength = data.Length - headerSize;
            var dataBuffer = new byte[dataLength];
            Buffer.BlockCopy(data, headerSize, dataBuffer, 0, dataLength);

            return await CreateSerializer(serializationMethod).Deserialize(dataBuffer);
        }

        private IMessageSerializer CreateSerializer(SerializationMethod serializationMethod)
        {
            return serializationMethod switch
            {
                SerializationMethod.Json => new JsonMessageSerializer(),
                _ => null!
            };
        }
    }
}
