using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IAndroidLocationService
    {
        void StartLocationUpdates();

        void StopLocationUpdates();

        event EventHandler<Location> LocationUpdated;
    }
}