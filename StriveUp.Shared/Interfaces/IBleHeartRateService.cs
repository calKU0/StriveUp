using Plugin.BLE.Abstractions.Contracts;
using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
