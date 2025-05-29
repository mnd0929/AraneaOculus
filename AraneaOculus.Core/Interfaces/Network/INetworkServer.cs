using AraneaOculus.Core.Models;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Core.Interfaces.Network
{
    public interface INetworkServer
    {
        bool IsWorks { get; }

        event EventHandler<Guid>? ClientConnected;

        event EventHandler<Guid>? ClientDisconnected;

        event EventHandler<DataReceivedEventArgs>? MessageReceived;

        Task StartAsync(ConnectionAddress address);

        Task SendAsync(Guid clientId, NetworkMessage message, CancellationToken cancellationToken = default);

        Task<Answer> SendRequestAsync(Guid clientId, Request request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

        List<Guid> GetClientIDs();

        ConnectionAddress GetClientConnectionAddress(Guid clientId);

        void Stop();
    }
}