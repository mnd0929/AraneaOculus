using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Diagnostics;
using AraneaOculus.Core.Models.Network;
using System.Net;

namespace AraneaOculus.Network.Endpoints
{
    public class TcpSocketServer
    {
        public readonly ConcurrentDictionary<Guid, TcpClient> Clients = new();

        public bool IsWorks => Listener?.Server.IsBound ?? false;

        public event Action<Guid, byte[]>? OnDataReceived;

        public event Action<Guid>? ClientConnected;

        public event Action<Guid>? ClientDisconnected;

        private TcpListener? Listener;

        private CancellationTokenSource TokenSource = new();

        public async Task StartAsync(ConnectionAddress address)
        {
            Listener = new TcpListener(address.Host, address.Port);
            Listener.Start();

            try
            {
                while (!TokenSource.IsCancellationRequested)
                {
                    var client = await Listener.AcceptTcpClientAsync(TokenSource.Token);
                    var clientId = Guid.NewGuid();
                    Clients.TryAdd(clientId, client);
                    ClientConnected?.Invoke(clientId);
                    _ = HandleClientAsync(clientId, client);
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task HandleClientAsync(Guid clientId, TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();

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

                    OnDataReceived?.Invoke(clientId, dataBuffer);
                }
            }
            catch (Exception) { }
            finally
            {
                Clients.TryRemove(clientId, out _);
                ClientDisconnected?.Invoke(clientId);
                client.Dispose();
            }
        }

        public async Task SendAsync(Guid clientId, byte[] data)
        {
            if (Clients.TryGetValue(clientId, out var client))
            {
                try
                {
                    Debug.WriteLine($"[{clientId}] Attempt to write {data.Length} bytes...");
                    var stream = client.GetStream();
                    // Добавление префикса длины
                    var lengthPrefix = BitConverter.GetBytes(data.Length);
                    await stream.WriteAsync(lengthPrefix, TokenSource.Token);
                    await stream.WriteAsync(data, TokenSource.Token);
                    Debug.WriteLine($"[{clientId}] Successfully written {data.Length} bytes");
                }
                catch
                {
                    Clients.TryRemove(clientId, out _);
                    ClientDisconnected?.Invoke(clientId);
                }
            }
        }

        public void Stop()
        {
            TokenSource.Cancel();
            Listener?.Stop();
            foreach (var client in Clients.Values)
                client.Dispose();
            Clients.Clear();
        }

        public TcpClient GetClientById(Guid clientId)
        {
            if (Clients.ContainsKey(clientId))
                return Clients[clientId];
            else return null!;
        }

        public ConnectionAddress GetClientConnectionAddress(Guid clientId)
        {
            IPEndPoint ipEndPoint = (IPEndPoint)GetClientById(clientId).Client.RemoteEndPoint!;
            int port = ipEndPoint.Port;
            string address = ipEndPoint.Address.ToString();

            return new ConnectionAddress(address, port);
        }
    }
}
