using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface ILocationTrackingService
    {
        event EventHandler<Location> LocationUpdated;

        Task StartAsync();

        Task StopAsync();

        Task<Location?> GetLastKnownLocationAsync();
    }
}