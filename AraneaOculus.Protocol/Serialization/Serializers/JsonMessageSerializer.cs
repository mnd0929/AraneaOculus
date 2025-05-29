using System.Text;
using Newtonsoft.Json;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Core.Interfaces.Network;

namespace AraneaOculus.Protocol.Serialization.Serializers
{
    internal class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        public byte[] Serialize(NetworkMessage message)
        {
            var json = JsonConvert.SerializeObject(message, SerializationSettings);

            return Encoding.UTF8.GetBytes(json);
        }

        public Task<NetworkMessage> Deserialize(byte[] data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data);
            var message = JsonConvert.DeserializeObject<NetworkMessage>(json, SerializationSettings);

            if (message is null)
            {
                throw new InvalidDataException("Failed to deserialize message");
            }

            return Task.FromResult(message);
        }
    }
}
