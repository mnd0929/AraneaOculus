using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Network.Endpoints
{
    public class TcpSocketClient : IDisposable
    {
        public event Action<byte[]>? OnDataReceived;

        public bool IsConnected => Client?.Connected ?? false;

        public event Action? OnConnected;

        public event Action? OnDisconnected;

        private readonly System.Net.Sockets.TcpClient Client = new();

        private CancellationTokenSource TokenSource = new();

        public async Task ConnectAsync(ConnectionAddress address)
        {
            await Client.ConnectAsync(address.Host, address.Port);
            _ = ReceiveLoopAsync();
            OnConnected?.Invoke();
        }

        public async Task SendAsync(byte[] data)
        {
            if (Client.Connected)
            {
                var stream = Client.GetStream();
                // Добавление префикса длины
                var lengthPrefix = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(lengthPrefix, TokenSource.Token);
                await stream.WriteAsync(data, TokenSource.Token);
            }
        }

        public void Disconnect()
        {
            TokenSource.Cancel();
            Client.Close();
            OnDisconnected?.Invoke();
        }

        public void Dispose()
        {
            Disconnect();
            Client.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task ReceiveLoopAsync()
        {
            try
            {
                var stream = Client.GetStream();

                while (!TokenSource.IsCancellationRequested)
                {
                    // Чтение префикса длины (4 байта)
                    var lengthBuffer = new byte[4];
                    try
                    {
                        await stream.ReadExactlyAsync(lengthBuffer, TokenSource.Token);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    if (messageLength <= 0)
                        continue;

                    var dataBuffer = new byte[messageLength];
                    try
                    {
                        await stream.ReadExactlyAsync(dataBuffer, TokenSource.Token);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    OnDataReceived?.Invoke(dataBuffer);
                }
            }
            catch (Exception) { }
            finally
            {
                Disconnect();
                Dispose();
            }
        }
    }
}
