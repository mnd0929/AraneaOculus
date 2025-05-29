using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Data;
using AraneaOculus.Core.Models.Network;
using AraneaOculus.Core.Utilities;
using AraneaOculus.Manager.Engine;
using AraneaOculus.Manager.Engine.Configuration;
using AraneaOculus.Manager.UI.View.ChartControllers;
using QRCoder;
using System.ComponentModel;

namespace AraneaOculus.Manager.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ConfigureNetworkInterfacesList();
            ConfigureDevicesList();
        }

        private ManagerController? Controller;

        private Dictionary<NetworkDevice, PropertyChangedRelay> Relays = new();

        private void ConfigureNetworkInterfacesList()
        {
            var interfaceEnumerator = NetworkUtils.GetLocalUnicastAddresses();
            while (interfaceEnumerator.MoveNext())
            {
                var item = interfaceEnumerator.Current;

                networkInterfacesComboBox.Items.Add(item.Address);
            }
            if (networkInterfacesComboBox.Items.Count > 0)
                networkInterfacesComboBox.SelectedIndex = networkInterfacesComboBox.Items.Count - 1;
        }

        private void ConfigureDevicesList()
        {
            devicesListView.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader { Text = "Sign ID", Width = 100, Tag = nameof(NetworkDevice.SignBasedIdentifier.Token) },
                new ColumnHeader { Text = "Sign Probability", Width = 40, Tag = nameof(NetworkDevice.SignBasedIdentifier.Probability) },
                new ColumnHeader { Text = "Connection ID", Width = 100, Tag = nameof(NetworkDevice.ConnectionId) },
                new ColumnHeader { Text = "Unique ID", Width = 50, Tag = nameof(NetworkDevice.DeviceId) },
                new ColumnHeader { Text = "IP", Width = 100, Tag = nameof(NetworkDevice.ConnectionAddress.Host) },
                new ColumnHeader { Text = "Port", Width = 50, Tag = nameof(NetworkDevice.ConnectionAddress.Port) },
                new ColumnHeader { Text = "MAC", Width = 100, Tag = nameof(NetworkDevice.MacAddress) },
                new ColumnHeader { Text = "Name", Width = 150, Tag = nameof(NetworkDevice.Info.Name) },
                new ColumnHeader { Text = "Vendor", Width = 150, Tag = nameof(NetworkDevice.Info.Vendor) },
                new ColumnHeader { Text = "Trust Level", Width = 100, Tag = nameof(NetworkDevice.CurrentTrustLevel) },
                new ColumnHeader { Text = "Detection Type", Width = 100, Tag = nameof(NetworkDevice.DetectionType) },
                new ColumnHeader { Text = "Packet Statistics", Width = 150, Tag = nameof(NetworkDevice.Info.PacketStatistics) },
                new ColumnHeader { Text = "Open Ports", Width = 100, Tag = nameof(NetworkDevice.Info.OpenPorts) },
            });
        }

        private async Task StartController()
        {
            string address = networkInterfacesComboBox.SelectedItem?.ToString() ?? "0.0.0.0";

            ConnectionAddress networkAddress = new ConnectionAddress(address, (int)portNumericUpDown.Value);
            Controller = new ManagerController(new ManagerConfiguration
            {
                HostAddress = networkAddress
            });

            Controller.SetSynchronizationContext(SynchronizationContext.Current!);
            Controller.Devices!.CollectionChanged += OnDevicesCollectionChanged;
            Controller.ConnectionsHistory!.CollectionChanged += OnConnectionHistoryCollectionChanged;
            Controller.PacketHistory!.CollectionChanged += OnPacketHistoryCollectionChanged;
            Controller.OnStatusChanged += OnControllerStatusChanged;
            await Controller.Start();

            powerButton.Text = "Stop";
            statusLabel.ForeColor = Color.Green;
            statusLabel.Text = $"Started at {networkAddress.Host}:{networkAddress.Port}";
        }

        private void StopController()
        {
            Controller!.Devices!.CollectionChanged -= OnDevicesCollectionChanged;
            Controller.ConnectionsHistory!.CollectionChanged -= OnConnectionHistoryCollectionChanged;
            Controller.PacketHistory!.CollectionChanged -= OnPacketHistoryCollectionChanged;
            Controller.OnStatusChanged -= OnControllerStatusChanged;
            Controller?.Stop();

            powerButton.Text = "Start";
            statusLabel.ForeColor = Color.Black;
            statusLabel.Text = $"Not started";
        }

        private string? GetColumnValue(ColumnHeader column, NetworkDevice device)
        {
            return column.Tag switch
            {
                nameof(NetworkDevice.SignBasedIdentifier.Token) => device?.SignBasedIdentifier?.Token.ToString() ?? "Unknown",
                nameof(NetworkDevice.SignBasedIdentifier.Probability) => $"{Math.Round(device?.SignBasedIdentifier?.Probability ?? 0, 3) * 100.0}%",
                nameof(NetworkDevice.ConnectionId) => device?.ConnectionId.ToString() ?? "Unknown",
                nameof(NetworkDevice.DeviceId) => device?.DeviceId?.ToString() ?? "Unknown",
                nameof(NetworkDevice.ConnectionAddress.Host) => device?.ConnectionAddress.Host.ToString(),
                nameof(NetworkDevice.ConnectionAddress.Port) => device?.ConnectionAddress.Port.ToString(),
                nameof(NetworkDevice.MacAddress) => device?.MacAddress,
                nameof(NetworkDevice.Info.Name) => device?.Info?.Name,
                nameof(NetworkDevice.Info.Vendor) => device?.Info?.Vendor,
                nameof(NetworkDevice.CurrentTrustLevel) => device?.CurrentTrustLevel.ToString(),
                nameof(NetworkDevice.DetectionType) => device?.DetectionType.ToString(),
                nameof(NetworkDevice.Info.OpenPorts) => string.Join(", ", device?.Info?.OpenPorts ?? new List<int>()),
                nameof(NetworkDevice.Info.PacketStatistics) =>
                    $"R:{device?.Info?.PacketStatistics?.Received} " +
                    $"S:{device?.Info?.PacketStatistics?.Sent} ",
                _ => string.Empty
            };
        }

        private void SetStatus(string text, Image image = null!, int progress = -1, ProgressBarStyle style = ProgressBarStyle.Blocks)
        {
            mainToolStripProgressBar.Value = progress < 0 ? mainToolStripProgressBar.Value : progress;
            mainToolStripProgressBar.Style = style;

            mainToolStripStatusLabel.Image = image == null ? mainToolStripStatusLabel.Image : image;
            mainToolStripStatusLabel.Text = text;
        }

        private void SetQrCodeData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                qrPictureBox.Image = null!;
            }
            else
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Default);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(128);
                qrPictureBox.Image = qrCodeImage;
            }
        }

        private void DetailedConnectionsTimelineChartRefresh()
        {
            if (detailedStatisticsDeviceComboBox.Items.Count > 0 && detailedStatisticsDeviceComboBox.SelectedItem != null)
            {
                Guid id = (Guid)detailedStatisticsDeviceComboBox.SelectedItem;

                if (Controller!.ConnectionsHistory!.ContainsKey(id))
                    DetailedConnectionsTimelineChartController.Display(detailedConnectionsTimelineChart, Controller!.ConnectionsHistory![id]);

                if (Controller!.PacketHistory!.ContainsKey(id))
                    DetailedNetworkLoadChartController.Display(detailedNetworkLoadTimelineChart, Controller!.PacketHistory![id]);
            }
        }

        private void OnControllerStatusChanged(object? sender, ManagerStatus e)
        {
            switch (e)
            {
                case ManagerStatus.UpdatingVendorData:
                    SetStatus("IEEE Standards Association Data Retrieval", image: Properties.Resources.wait17, style: ProgressBarStyle.Marquee);
                    break;

                case ManagerStatus.LaunchingNetworkListening:
                    SetStatus("Launching Network Listening", image: Properties.Resources.wait17);
                    break;

                case ManagerStatus.TerminatingNetworkListening:
                    SetStatus("Terminating Network Listening", image: Properties.Resources.wait17);
                    break;

                case ManagerStatus.LaunchingNetworkScanning:
                    SetStatus("Launching Network Scanning", image: Properties.Resources.parallel_tasks17);
                    break;

                case ManagerStatus.TerminatingNetworkScanning:
                    SetStatus("Terminating Network Scanning", image: Properties.Resources.parallel_tasks17);
                    break;

                case ManagerStatus.NetworkMonitoring:
                    SetStatus("Network Monitoring", image: Properties.Resources.monitoring17);
                    break;

                case ManagerStatus.Analysis:
                    SetStatus("Data Analysis", image: Properties.Resources.wait17);
                    break;

                case ManagerStatus.Inaction:
                    SetStatus("Inaction", Properties.Resources.cloud_warning17);
                    break;
            }
        }

        private async void OnPowerButtonClick(object sender, EventArgs e)
        {
            powerButton.Enabled = false;

            if (Controller?.IsWorks ?? false)
            {
                if (MessageBox.Show(
                    "Disabling the AraneaOculus Manager even for a couple of minutes is a potentially dangerous action. " +
                    "Before doing this, it is recommended to make sure that all devices except the host computer are " +
                    "disconnected from the router. Are you sure you want to continue?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    StopController();

                    networkInterfacesComboBox.Enabled = true;
                    portNumericUpDown.Enabled = true;

                    SetQrCodeData(null!);
                }
            }
            else
            {
                networkInterfacesComboBox.Enabled = false;
                portNumericUpDown.Enabled = false;

                await StartController();

                SetQrCodeData($"{networkInterfacesComboBox.Text}:{portNumericUpDown.Value}");
            }

            powerButton.Enabled = true;
        }

        private void OnDevicesListRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var columns = devicesListView.Columns.Cast<ColumnHeader>()
                                                 .OrderBy(c => c.DisplayIndex)
                                                 .ToList();

            if (Controller?.Devices == null || e.ItemIndex < 0 || e.ItemIndex >= Controller.Devices.Count)
            {
                var invalidItem = new ListViewItem("Unknown Device");
                while (invalidItem.SubItems.Count < columns.Count)
                    invalidItem.SubItems.Add(string.Empty);
                e.Item = invalidItem;
                return;
            }

            var device = Controller.Devices[e.ItemIndex];
            var item = new ListViewItem();

            for (int i = 0; i < columns.Count; i++)
            {
                string value = GetColumnValue(columns[i], device!) ?? string.Empty;
                if (i == 0)
                    item.Text = value;
                else
                    item.SubItems.Add(value);
            }

            while (item.SubItems.Count < columns.Count)
                item.SubItems.Add(string.Empty);

            item.BackColor = device?.CurrentTrustLevel switch
            {
                TrustLevel.Trust => Color.LightGreen,
                TrustLevel.PotentialDanger => Color.Orange,
                TrustLevel.NoTrust => Color.Tomato,
                _ => Color.Tomato
            };

            e.Item = item;
        }

        private void OnDevicesListColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            devicesListView.Invalidate();
        }

        private void OnPacketHistoryCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NetworkLoadChartController.Display(networkLoadChart, Controller!.PacketHistory!);

            DetailedConnectionsTimelineChartRefresh();
        }

        private void OnConnectionHistoryCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ConnectionNumberChartController.Display(connectionsNumberChart, Controller!.ConnectionsHistory!);

            DetailedConnectionsTimelineChartRefresh();
        }

        private void OnDevicesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            devicesListView.BeginUpdate();
            devicesListView.VirtualListSize = Controller!.Devices!.Count;

            if (e.NewItems != null)
            {
                foreach (NetworkDevice item in e.NewItems)
                {
                    Relays[item] = new PropertyChangedRelay(item, UpdateItemView!);
                }
            }
            if (e.OldItems != null)
            {
                foreach (NetworkDevice item in e.OldItems)
                {
                    if (Relays.TryGetValue(item, out var relay))
                    {
                        relay.Dispose();
                        Relays.Remove(item);
                    }
                }
            }

            Controller!.ConnectionsHistory!.ToList().ForEach(entry =>
            {
                if (!detailedStatisticsDeviceComboBox.Items.Contains(entry.Key))
                    detailedStatisticsDeviceComboBox.Items.Add(entry.Key);
            });
            devicesListView.EndUpdate();
        }

        private void UpdateItemView(object sender, PropertyChangedEventArgs e)
        {
            NetworkDevice? device = null;

            if (sender is NetworkDevice d)
            {
                device = d;
            }
            else if (sender is NetworkDeviceInfo info)
            {
                device = Controller!.Devices!.FirstOrDefault(x => x.Info == info);
            }
            else if (sender is PacketStatisticsData stats)
            {
                device = Controller!.Devices!.FirstOrDefault(x => x.Info.PacketStatistics == stats);
            }

            if (device == null) return;

            int index = Controller!.Devices!.IndexOf(device);

            if (devicesListView.InvokeRequired)
            {
                devicesListView.Invoke(new Action(() => UpdateItemView(sender, e)));
                return;
            }

            if (index >= 0 && index < devicesListView.VirtualListSize)
            {
                var bounds = devicesListView.GetItemRect(index);
                devicesListView.Invalidate(bounds);
            }
        }

        private void detailedStatisticsDeviceComboBoxSelectedIndexChanged(object sender, EventArgs e) => DetailedConnectionsTimelineChartRefresh();
    }
}
