using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IActivityTrackingService
    {
        event EventHandler<Location> LocationUpdated;

        Task StartAsync(bool isIndoor = false, bool startNow = false);

        Task StopAsync();

        Task PauseAsync();

        Task ResumeAsync(bool isIndoor = false);

        Task<Location?> GetLastKnownLocationAsync();

        bool IsBackgroundActivityAllowed();
    }
}