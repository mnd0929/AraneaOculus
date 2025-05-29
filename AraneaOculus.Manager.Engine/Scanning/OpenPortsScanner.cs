using AraneaOculus.Core.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;

namespace AraneaOculus.Manager.Engine.Scanning
{
    public class OpenPortsScanner
    {
        public readonly IPAddress? Address;

        public ObservableCollection<int>? OpenPorts;

        private SynchronizationContext? Context;

        public int ParallelTaskLimit { get; set; }

        public int Timeout { get; set; }

        public int Interval { get; set; }

        private CancellationTokenSource? TokenSource;

        public OpenPortsScanner(IPAddress address, int interval = 120000, int parallelTaskLimit = 16, int timeout = 300)
        {
            Address = address;
            OpenPorts = new ObservableCollection<int>();
            ParallelTaskLimit = parallelTaskLimit;
            Interval = interval;
            Timeout = timeout;
        }

        public void SetSynchronizationContext(SynchronizationContext context)
        {
            Context = context;
        }

        public void Start()
        {
            TokenSource = new CancellationTokenSource();
            var ports = Enumerable.Range(1, 1024).ToList();

            _ = StartLoop(ports);
        }

        public void Stop()
        {
            TokenSource?.Cancel();

            if (Context != null)
                Context?.Post(_ => OpenPorts!.Clear(), null);
            else OpenPorts!.Clear();
        }

        private async Task StartLoop(List<int> ports)
        {
            while (!TokenSource!.IsCancellationRequested)
            {
                try
                {
                    Debug.WriteLine($"[PORTS@{Address!.ToString()}] StartScan");
                    await ScanPorts(ports);
                    Debug.WriteLine($"[PORTS@{Address!.ToString()}] FinScan");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception in ScanPorts: {ex}");
                }

                await Task.Delay(Interval);
            }
        }

        private async Task ScanPorts(List<int> ports)
        {
            int processedCount = 0;
            using (var semaphore = new SemaphoreSlim(ParallelTaskLimit))
            {
                var tasks = ports.Select(async port =>
                {
                    await semaphore.WaitAsync(TokenSource!.Token).ConfigureAwait(false);
                    try
                    {
                        bool isOpen = await NetworkUtils.CheckPortAsync(Address!.ToString(), port, Timeout).ConfigureAwait(false);

                        Context?.Post(_ =>
                        {
                            if (isOpen)
                            {
                                if (!OpenPorts!.Contains(port))
                                {
                                    OpenPorts.Add(port);
                                }
                            }
                            else
                            {
                                if (OpenPorts!.Contains(port))
                                {
                                    OpenPorts.Remove(port);
                                }
                            }
                        }, null);
                    }
                    finally
                    {
                        semaphore.Release();
                        if (Interlocked.Increment(ref processedCount) % ParallelTaskLimit == 0)
                            Debug.WriteLine(
                              $"[PORTS@{Address!.ToString()}] Scan progress [{processedCount}/{ports.Count}, open:{OpenPorts!.Count}]"
                            );
                    }
                }).ToList();

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
    }
}
