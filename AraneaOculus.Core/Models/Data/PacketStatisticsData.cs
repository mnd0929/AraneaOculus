using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AraneaOculus.Core.Models.Data
{
    public class PacketStatisticsData : INotifyPropertyChanged
    {
        private long _sent;

        public long Sent
        {
            get { return _sent; }
            set { _sent = value; OnPropertyChanged(); }
        }

        private long _received;

        public long Received
        {
            get { return _received; }
            set { _received = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
