using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Interfaces.Network;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Network.Endpoints;

namespace AraneaOculus.Network.Shells
{
    public class NetworkClientShell : INetworkClient
    {
        public bool IsConnected => TcpClient?.IsConnected ?? false;

        public event EventHandler? Connected;

        public event EventHandler? Disconnected;

        public event EventHandler<NetworkMessage>? MessageReceived;

        private TcpSocketClient? TcpClient;

        private INetworkPacketSerializer Serializer;

        private SerializationMethod SerializationMethod;

        public NetworkClientShell(INetworkPacketSerializer serializer, SerializationMethod serializationMethod)
        {
            Serializer = serializer;
            SerializationMethod = serializationMethod;
        }

        public async Task ConnectAsync(ConnectionAddress address, CancellationToken cancellationToken = default)
        {
            TcpClient = new TcpSocketClient();
            TcpClient.OnDataReceived += TcpClientOnDataReceived;
            TcpClient.OnConnected += TcpClientOnConnected;
            TcpClient.OnDisconnected += TcpClientOnDisconnected;
            await TcpClient.ConnectAsync(address);
        }

        public void Disconnect()
        {
            if (TcpClient != null)
            {
                TcpClient.OnDataReceived -= TcpClientOnDataReceived;
                TcpClient.OnConnected -= TcpClientOnConnected;
                TcpClient.OnDisconnected -= TcpClientOnDisconnected;
                TcpClient.Disconnect();
                TcpClient.Dispose();
                TcpClient = null;
            }
        }

        private void TcpClientOnDisconnected() => Disconnected?.Invoke(this, null!);

        private void TcpClientOnConnected() => Connected?.Invoke(this, null!);

        private async void TcpClientOnDataReceived(byte[] data)
        {
            var message = await Serializer.Deserialize(data);
            MessageReceived?.Invoke(this, message);
        }

        public async Task SendAsync(NetworkMessage message, CancellationToken cancellationToken = default)
        {
            if (TcpClient == null)
            {
                throw new InvalidOperationException("Attempt to send data via uninitialized client");
            }
            
            byte[] bytes = Serializer.Serialize(message, SerializationMethod);
            await TcpClient.SendAsync(bytes);
        }

        public async Task<Answer> SendRequestAsync(Request request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            if (TcpClient == null)
            {
                throw new InvalidOperationException("Attempt to send request via uninitialized client");
            }

            CancellationTokenSource? timeoutCts = timeout.HasValue ? new CancellationTokenSource(timeout.Value) : null;
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts?.Token ?? CancellationToken.None);

            var tcs = new TaskCompletionSource<NetworkMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void Handler(byte[] data)
            {
                try
                {
                    var response = await Serializer.Deserialize(data).ConfigureAwait(false);
                    if (response != null && response.Id == request.Id)
                    {
                        tcs.TrySetResult(response!);
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

            TcpClient.OnDataReceived += Handler;
            try
            {
                await SendAsync(request, linkedCts.Token).ConfigureAwait(false);

                using (linkedCts.Token.Register(() => tcs.TrySetCanceled(linkedCts.Token)))
                {
                    return (Answer)await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                TcpClient.OnDataReceived -= Handler;
                timeoutCts?.Dispose();
            }
        }
    }
}
