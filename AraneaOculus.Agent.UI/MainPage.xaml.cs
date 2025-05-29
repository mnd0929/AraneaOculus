using AraneaOculus.Core.Interfaces;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Agent.UI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private IBackgroundService? Service;

        private bool IsConnected;

        private async void ConnectButtonClicked(object sender, EventArgs e)
        {
            if (IsConnected)
                Disconnect();
            else await Connect(AddressEntry.Text, PortEntry.Text);
        }

        private void ScanButtonClicked(object sender, EventArgs e)
        {
            if (cameraBarcodeReaderView.IsDetecting)
                StopScan();
            else StartScan();
        }

        private void cameraBarcodeReaderViewBarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            string[] parts = e.Results.FirstOrDefault()!.Value.Split(':');
            Dispatcher.Dispatch(async () =>
            {
                StopScan();
                await Connect(parts[0], parts[1]);
            });
        }

        private async Task Connect(string address, string port)
        {
            ConnectionAddress networkAddress = new ConnectionAddress(address, Convert.ToInt32(port));

            Service = Application.Current?.Handler.MauiContext?.Services.GetService<IBackgroundService>();
            await Service?.Start(networkAddress)!;

            ScanButton.IsVisible = false;
            AddressEntry.IsVisible = false;
            PortEntry.IsVisible = false;

            IsConnected = true;
            ConnectButton.Text = "Disconnect";
        }

        private void Disconnect()
        {
            Service?.Stop();

            ScanButton.IsVisible = true;
            AddressEntry.IsVisible = true;
            PortEntry.IsVisible = true;

            IsConnected = false;
            ConnectButton.Text = "Connect";
        }

        private void StartScan()
        {
            cameraBarcodeReaderView.IsDetecting = true;
            cameraBarcodeReaderBorder.IsVisible = true;
            ScanButton.Text = "Stop Scan";
        }

        private void StopScan()
        {
            cameraBarcodeReaderView.IsDetecting = false;
            cameraBarcodeReaderBorder.IsVisible = false;
            ScanButton.Text = "Start Scan";
        }
    }
}
