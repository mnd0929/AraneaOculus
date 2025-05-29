using AraneaOculus.Core.Enums;
using AraneaOculus.Core.Models.Identification;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AraneaOculus.Core.Models.Network
{
    public class NetworkDevice : INotifyPropertyChanged
    {
        public NetworkDevice() => ConnectionId = Guid.NewGuid();

        public NetworkDevice(Guid guid) => ConnectionId = guid;

        private ConnectionAddress? _connectionAddress;

        public ConnectionAddress ConnectionAddress
        {
            get => _connectionAddress!;
            set { _connectionAddress = value; OnPropertyChanged(); }
        }

        private Identifier? _signBasedIdentifier;

        public Identifier? SignBasedIdentifier
        {
            get { return _signBasedIdentifier; }
            set { _signBasedIdentifier = value; OnPropertyChanged(); }
        }


        private Guid _сonnectionId;

        public Guid ConnectionId
        {
            get { return _сonnectionId; }
            set { _сonnectionId = value; OnPropertyChanged(); }
        }

        private string? _deviceId;

        public string? DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; OnPropertyChanged(); }
        }

        private string? _macAddress;

        public string MacAddress
        {
            get => _macAddress!;
            set { _macAddress = value; OnPropertyChanged(); }
        }

        private TrustLevel _currentTrustLevel;

        public TrustLevel CurrentTrustLevel
        {
            get => _currentTrustLevel;
            set { _currentTrustLevel = value; OnPropertyChanged(); }
        }

        private DetectionType _detectionType;

        public DetectionType DetectionType
        {
            get { return _detectionType; }
            set { _detectionType = value; OnPropertyChanged(); }
        }

        private NetworkDeviceInfo? _info = new();

        public NetworkDeviceInfo Info
        {
            get => _info!;
            set { _info = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
