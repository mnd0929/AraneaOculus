using AraneaOculus.Core.Models.Identification;
using AraneaOculus.Core.Models.Network;
using System.Diagnostics;

namespace AraneaOculus.Manager.Engine
{
    public class IdentifierStorage
    {
        private Dictionary<Guid, DeviceSigns> SignsStorage = new Dictionary<Guid, DeviceSigns>();
        private readonly object _lock = new object();

        public Identifier GetIdentifier(NetworkDevice networkDevice)
        {
            DeviceSigns deviceSigns = new DeviceSigns(networkDevice);

            return GetIdentifier(deviceSigns);
        }

        public Identifier GetIdentifier(DeviceSigns deviceSigns)
        {
            KeyValuePair<Guid, DeviceSigns> bestMatch = new KeyValuePair<Guid, DeviceSigns>();
            double maxProbability = 0.0;

            lock (_lock)
            {
                foreach (var signs in SignsStorage)
                {
                    double probability = CalculateProbability(signs.Value, deviceSigns);
                    if (probability == 1.0)
                    {
                        bestMatch = signs;
                        maxProbability = probability;
                        break;
                    }
                    else if (probability > maxProbability)
                    { 
                        bestMatch = signs;
                        maxProbability = probability;
                    }
                }

                if (maxProbability == 0.0)
                {
                    Guid id = Guid.NewGuid();
                    SignsStorage.Add(id, deviceSigns);

                    return new Identifier(id, 1.0);
                }
                else if (maxProbability < 0.5)
                {
                    return new Identifier(Guid.NewGuid(), 0.0);
                }
                else return new Identifier(bestMatch.Key, maxProbability);
            }
        }

        private double CalculateProbability(DeviceSigns firstDeviceSigns, DeviceSigns secondDeviceSigns)
        {
            if (firstDeviceSigns.UniqueIdentifier != null && secondDeviceSigns.UniqueIdentifier != null && 
                    firstDeviceSigns.UniqueIdentifier == secondDeviceSigns.UniqueIdentifier)
                return 1.0;

            double secondarySignsWeight =
                (firstDeviceSigns.Name != null && secondDeviceSigns.Name != null ? 1.0 : 0.0) +
                (firstDeviceSigns.Address != null && secondDeviceSigns.Address != null ? 2.0 : 0.0) +
                (firstDeviceSigns.MacAddress != null && secondDeviceSigns.MacAddress != null ? 3.0 : 0.0);

            if (secondarySignsWeight == 0)
                return 0.0;

            double matchedSecondarySignsWeight = 0.0;

            // Вес имени устройства - 1
            if (firstDeviceSigns.Name != null && secondDeviceSigns.Name != null &&
                firstDeviceSigns.Name == secondDeviceSigns.Name)
                matchedSecondarySignsWeight += 1.0;

            // Вес IP адреса устройства - 2
            if (firstDeviceSigns.Address != null && secondDeviceSigns.Address != null &&
                firstDeviceSigns.Address == secondDeviceSigns.Address)
                matchedSecondarySignsWeight += 2.0;

            // Вес MAC адреса устройства - 3
            if (firstDeviceSigns.MacAddress != null && secondDeviceSigns.MacAddress != null &&
                firstDeviceSigns.MacAddress == secondDeviceSigns.MacAddress)
                matchedSecondarySignsWeight += 3.0;

            Debug.WriteLine($"*** secondarySignsWeight: {secondarySignsWeight}; matchedSecondarySignsWeight: {matchedSecondarySignsWeight}");
            Debug.WriteLine($"{firstDeviceSigns.Name} ?? {secondDeviceSigns.Name}");
            Debug.WriteLine($"{firstDeviceSigns.Address} ?? {secondDeviceSigns.Address}");
            Debug.WriteLine($"{firstDeviceSigns.MacAddress} ?? {secondDeviceSigns.MacAddress}");
            Debug.WriteLine($"*** PR: {matchedSecondarySignsWeight / secondarySignsWeight}");

            return matchedSecondarySignsWeight / secondarySignsWeight;
        }
    }
}
