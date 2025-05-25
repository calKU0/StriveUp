using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;

namespace StriveUp.MAUI.Services
{
    public class BleHeartRateService : IBleHeartRateService
    {
        private readonly IAdapter _adapter;
        private IDevice _device;
        private readonly IBluetoothLE _ble;
        private ICharacteristic _hrCharacteristic;
        private DateTime _lastHeartRateUpdate = DateTime.MinValue;

        public event Action<int> OnHeartRateChanged;

        public bool IsConnected => _device?.State == DeviceState.Connected;

        private readonly Guid HR_SERVICE_UUID = Guid.Parse("0000180D-0000-1000-8000-00805f9b34fb");
        private readonly Guid HR_MEASUREMENT_UUID = Guid.Parse("00002A37-0000-1000-8000-00805f9b34fb");
        private readonly Guid GAP_SERVICE_UUID = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb");
        private readonly Guid DEVICE_NAME_CHARACTERISTIC_UUID = Guid.Parse("00002A00-0000-1000-8000-00805f9b34fb");

        public BleHeartRateService()
        {
            _ble = CrossBluetoothLE.Current;
            _adapter = _ble.Adapter;
        }

        public List<BluetoothDeviceDto> GetAvailableDevicesAsync()
        {
            var availableDevices = new List<BluetoothDeviceDto>();
            foreach (var bleDevice in _adapter.BondedDevices)
            {
                availableDevices.Add(new BluetoothDeviceDto
                {
                    Name = bleDevice.Name,
                    Id = bleDevice.Id.ToString(),
                });
            }

            return availableDevices;
        }

        public async Task<bool> ConnectAsync(string id)
        {
            try
            {
                var target = _adapter.BondedDevices.FirstOrDefault(d => d.Id.ToString().Equals(id, StringComparison.OrdinalIgnoreCase));

                if (target == null)
                    return false;

                _device = target;

                await _adapter.ConnectToDeviceAsync(_device);

                var service = await _device.GetServiceAsync(HR_SERVICE_UUID);
                _hrCharacteristic = await service.GetCharacteristicAsync(HR_MEASUREMENT_UUID);

                _hrCharacteristic.ValueUpdated += (s, e) =>
                {
                    if (e.Characteristic.Value.Length > 1)
                    {
                        if ((DateTime.Now - _lastHeartRateUpdate).TotalSeconds >= 1)
                        {
                            int hr = e.Characteristic.Value[1];
                            OnHeartRateChanged?.Invoke(hr);
                        }
                    }
                };

                await _hrCharacteristic.StartUpdatesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex);
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_device != null && IsConnected)
            {
                await _adapter.DisconnectDeviceAsync(_device);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_hrCharacteristic != null)
                {
                    _hrCharacteristic.StopUpdatesAsync();
                    _hrCharacteristic = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BLE Disconnect Error: {ex.Message}");
            }
        }
    }
}