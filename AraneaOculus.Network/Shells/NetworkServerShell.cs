using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Interfaces.Network;
using AraneaOculus.Core.Models;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Network.Endpoints;

namespace AraneaOculus.Network.Shells
{
    public class NetworkServerShell : INetworkServer
    {
        public bool IsWorks => TcpServer?.IsWorks ?? false;

        public event EventHandler<Guid>? ClientConnected;

        public event EventHandler<Guid>? ClientDisconnected;

        public event EventHandler<DataReceivedEventArgs>? MessageReceived;

        public event EventHandler<Exception>? ErrorOccurred;

        private TcpSocketServer? TcpServer;

        private INetworkPacketSerializer Serializer;

        private SerializationMethod SerializationMethod;

        public NetworkServerShell(INetworkPacketSerializer serializer, SerializationMethod serializationMethod)
        {
            Serializer = serializer;
            SerializationMethod = serializationMethod;
        }

        public async Task StartAsync(ConnectionAddress address)
        {
            TcpServer = new TcpSocketServer();
            TcpServer.OnDataReceived += TcpServerOnDataReceived;
            TcpServer.ClientDisconnected += id => ClientDisconnected?.Invoke(this, id);
            TcpServer.ClientConnected += id => ClientConnected?.Invoke(this, id);
            await TcpServer.StartAsync(address);
        }

        private async void TcpServerOnDataReceived(Guid clientId, byte[] data)
        {
            NetworkMessage message = await Serializer.Deserialize(data);

            DataReceivedEventArgs dataReceivedEventArgs = new DataReceivedEventArgs 
            {
                ClientId = clientId,
                Message = message
            };

            MessageReceived?.Invoke(this, dataReceivedEventArgs);
        }

        public async Task SendAsync(Guid clientId, NetworkMessage message, CancellationToken cancellationToken = default)
        {
            if (TcpServer == null)
                throw new InvalidOperationException("Attempting to transport via an inactive server");

            byte[] bytes = Serializer.Serialize(message, SerializationMethod);
            await TcpServer?.SendAsync(clientId, bytes)!;
        }

        public async Task<Answer> SendRequestAsync(Guid clientId, Request request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            CancellationTokenSource? timeoutCts = timeout.HasValue ? new CancellationTokenSource(timeout.Value) : null;

            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, timeoutCts?.Token ?? CancellationToken.None);

            var tcs = new TaskCompletionSource<Answer>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void Handler(Guid senderId, byte[] data)
            {
                if (!senderId.Equals(clientId))
                    return;

                try
                {
                    NetworkMessage message = await Serializer.Deserialize(data).ConfigureAwait(false);
                    if (message is Answer answer)
                    {
                        tcs.TrySetResult(answer);
                    }
                }
                catch (OperationCanceledException)
                {
                    tcs.TrySetCanceled(linkedCts.Token);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            TcpServer!.OnDataReceived += Handler;
            try
            {
                await SendAsync(clientId, request, linkedCts.Token).ConfigureAwait(false);

                using (linkedCts.Token.Register(() => tcs.TrySetCanceled(linkedCts.Token)))
                {
                    return await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                TcpServer.OnDataReceived -= Handler;
                timeoutCts?.Dispose();
            }
        }

        public void Stop()
        {
            TcpServer!.OnDataReceived -= TcpServerOnDataReceived;
            TcpServer?.Stop();
        }

        public List<Guid> GetClientIDs() => TcpServer?.Clients.Keys.ToList() ?? new List<Guid>();

        public ConnectionAddress GetClientConnectionAddress(Guid clientId) => TcpServer?.GetClientConnectionAddress(clientId)!;
    }
}
