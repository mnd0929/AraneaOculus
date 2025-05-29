using AraneaOculus.Core.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;

namespace AraneaOculus.Manager.Engine.Scanning
{
    public class NetworkScanner
    {
        public readonly IPAddress? Address;

        public readonly IPAddress? StartAddress;

        public readonly IPAddress? StopAddress;

        public ObservableCollection<IPAddress> OnlineDevices;

        private List<IPAddress> PossibleAddresses;

        private SynchronizationContext? Context;

        public int ParallelTaskLimit { get; set; }

        public int Timeout { get; set; }

        public int Interval { get; set; }

        private CancellationTokenSource? TokenSource;

        public NetworkScanner(IPAddress localAddress, int inerval = 4000, int parallelTaskLimit = 1536, int timeout = 2000)
        {
            Address = localAddress;
            StartAddress = NetworkUtils.CalculateStartAddress(Address);
            StopAddress = NetworkUtils.CalculateStopAddress(Address);
            OnlineDevices = new ObservableCollection<IPAddress>();
            PossibleAddresses = new List<IPAddress>();
            Interval = inerval;
            ParallelTaskLimit = parallelTaskLimit;
            Timeout = timeout;
        }

        public void SetSynchronizationContext(SynchronizationContext context)
        {
            Context = context;
        }

        public void Start()
        {
            TokenSource = new CancellationTokenSource();
            List<IPAddress> addresses = NetworkUtils.GenerateIPAddressesInRange(StartAddress!, StopAddress!).ToList();

            _ = StartLoop(addresses, PossibleAddresses, true);
            _ = StartLoop(PossibleAddresses, OnlineDevices);
        }

        public void Stop()
        {
            TokenSource?.Cancel();

            if (Context != null)
                Context?.Post(_ => OnlineDevices.Clear(), null);
            else OnlineDevices.Clear();
        }

        private async Task StartLoop(IList<IPAddress> addresses, IList<IPAddress> fillableList, bool onlyAdd = false)
        {
            while (!TokenSource!.IsCancellationRequested)
            {
                Debug.WriteLine($"*** START SCAN ({addresses.Count} addresses)");
                try
                {
                    await ScanAddresses(addresses.ToList(), fillableList, TokenSource.Token, onlyAdd);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception in ScanAddresses: {ex}");
                }
                Debug.WriteLine("*** END SCAN");

                await Task.Delay(Interval);
            }
        }

        private async Task ScanAddresses(List<IPAddress> addresses, IList<IPAddress> fillableList, CancellationToken token, bool onlyAdd = false)
        {
            int processedCount = 0;
            using (var semaphore = new SemaphoreSlim(ParallelTaskLimit))
            {
                var tasks = addresses.Select(async ip =>
                {
                    await semaphore.WaitAsync(token).ConfigureAwait(false);
                    try
                    {
                        bool isActive = await NetworkUtils.PingIPAsync(ip, Timeout).ConfigureAwait(false);

                        Context?.Post(_ =>
                        {
                            if (isActive)
                            {
                                if (!OnlineDevices.Contains(ip))
                                {
                                    fillableList.Add(ip);
                                    Debug.WriteLine($"[ADD] {ip}");
                                }
                            }
                            else
                            {
                                if (fillableList.Contains(ip) && !onlyAdd)
                                {
                                    fillableList.Remove(ip);
                                    Debug.WriteLine($"[REMOVE] {ip}");
                                }
                            }
                        }, null);
                    }
                    catch { }
                    finally
                    {
                        if (Interlocked.Increment(ref processedCount) % ParallelTaskLimit == 0)
                            Debug.WriteLine(
                              $"Scan progress [{processedCount}/{addresses.Count}, online:{fillableList.Count}]"
                            );
                        semaphore.Release();
                    }
                }).ToList();

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

    }
}