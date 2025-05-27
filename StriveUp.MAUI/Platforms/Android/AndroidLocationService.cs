using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using StriveUp.Shared.Interfaces;
using System;
using StriveUp.MAUI;
using LocationMaui = Microsoft.Maui.Devices.Sensors.Location;
using LocationRequest = Android.Gms.Location.LocationRequest;
using AndroidLocation = Android.Locations;
using AndroidApp = Android.App.Application;

namespace StriveUp.MAUI.Platforms.Android
{
    public class AndroidLocationService : Java.Lang.Object, IAndroidLocationService
    {
        private readonly IFusedLocationProviderClient fusedLocationProviderClient;
        private LocationCallback locationCallback;

        public event EventHandler<LocationMaui> LocationUpdated;

        public AndroidLocationService()
        {
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(AndroidApp.Context);
        }

        public void StartLocationUpdates()
        {
            var locationRequest = new LocationRequest.Builder(Priority.PriorityHighAccuracy)
                .SetIntervalMillis(5000)
                .SetGranularity(Granularity.GranularityFine)
                .SetMinUpdateIntervalMillis(1000)
                .SetPriority(Priority.PriorityHighAccuracy)
                .SetWaitForAccurateLocation(true)
                .Build();

            locationCallback = new LocationCallbackImpl(this);

            fusedLocationProviderClient.RequestLocationUpdates(locationRequest, locationCallback, Looper.MainLooper);
        }

        public void StopLocationUpdates()
        {
            if (locationCallback != null)
            {
                fusedLocationProviderClient.RemoveLocationUpdates(locationCallback);
                locationCallback.Dispose();
            }
        }

        private void OnLocationResult(AndroidLocation.Location loc)
        {
            if (loc != null)
            {
                // Convert Android.Locations.Location to Microsoft.Maui.Devices.Sensors.Location
                var newLocation = ConvertToMauiLocation(loc);

                LocationUpdated?.Invoke(this, newLocation);
            }
        }

        private class LocationCallbackImpl : LocationCallback
        {
            private readonly AndroidLocationService parent;

            public LocationCallbackImpl(AndroidLocationService parent)
            {
                this.parent = parent;
            }

            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                var loc = result.LastLocation;
                if (loc != null)
                    parent.OnLocationResult(loc);
            }
        }

        public async Task<LocationMaui?> GetCurrentLocationAsync()
        {
            try
            {
                var loc = await fusedLocationProviderClient.GetLastLocationAsync();

                if (loc != null)
                {
                    return ConvertToMauiLocation(loc);
                }
            }
            catch
            {
            }

            return null;
        }

        private LocationMaui ConvertToMauiLocation(AndroidLocation.Location loc)
        {
            var newLocation = new LocationMaui(
                loc.Latitude,
                loc.Longitude,
                loc.HasAltitude ? loc.Altitude : 0
            )
            {
                Accuracy = loc.HasAccuracy ? loc.Accuracy : 0,
                Timestamp = loc.Time > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(loc.Time) : DateTimeOffset.Now,
                Speed = loc.HasSpeed ? loc.Speed : 0f
            };

            return newLocation;
        }
    }
}