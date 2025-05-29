using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Core.Interfaces.Network
{
    public interface INetworkClient
    {
        bool IsConnected { get; }

        event EventHandler? Connected;

        event EventHandler? Disconnected;

        event EventHandler<NetworkMessage>? MessageReceived;

        Task ConnectAsync(ConnectionAddress address, CancellationToken cancellationToken = default);

        void Disconnect();

        Task SendAsync(NetworkMessage message, CancellationToken cancellationToken = default);

        Task<Answer> SendRequestAsync(Request request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
    }
}