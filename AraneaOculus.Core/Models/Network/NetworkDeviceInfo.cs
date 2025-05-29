using AraneaOculus.Core.Models.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AraneaOculus.Core.Models.Network
{
    public class NetworkDeviceInfo : INotifyPropertyChanged
    {
        private PacketStatisticsData? _packetStatistics;

        public PacketStatisticsData? PacketStatistics
        {
            get => _packetStatistics;
            set { _packetStatistics = value; OnPropertyChanged(); }
        }

        private List<int>? _openPorts;

        public List<int>? OpenPorts
        {
            get => _openPorts;
            set { _openPorts = value; OnPropertyChanged(); }
        }

        private string? _name;

        public string Name
        {
            get => _name!;
            set { _name = value; OnPropertyChanged(); }
        }

        private string? _vendor;

        public string? Vendor
        {
            get { return _vendor; }
            set { _vendor = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
