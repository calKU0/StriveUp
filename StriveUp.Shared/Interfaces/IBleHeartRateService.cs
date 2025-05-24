using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface IBleHeartRateService
    {
        event Action<int> OnHeartRateChanged;

        List<BluetoothDeviceDto> GetAvailableDevicesAsync();

        Task<bool> ConnectAsync(string deviceId);

        void Disconnect();
    }
}