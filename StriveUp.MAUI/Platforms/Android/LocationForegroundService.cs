using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.Devices.Sensors;
using System;

namespace StriveUp.MAUI.Platforms.Android
{
    [Service(ForegroundServiceType = ForegroundService.TypeLocation)]
    public class LocationForegroundService : Service
    {
        private IFusedLocationProviderClient _fusedLocationProviderClient;
        private LocationCallback _locationCallback;

        public static event EventHandler<Location> LocationUpdated;

        public override void OnCreate()
        {
            base.OnCreate();
            _fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            _locationCallback = new LocationCallbackImpl();
            CreateNotificationChannel();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForeground(1, BuildNotification());
            StartLocationUpdates();
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopLocationUpdates();
        }

        public override IBinder OnBind(Intent intent) => null;

        private void StartLocationUpdates()
        {
            var locationRequest = new LocationRequest.Builder(Priority.PriorityBalancedPowerAccuracy, 5000)
                .SetMinUpdateIntervalMillis(3000)
                .Build();

            _fusedLocationProviderClient.RequestLocationUpdates(locationRequest, _locationCallback, Looper.MainLooper);
        }

        private void StopLocationUpdates()
        {
            if (_locationCallback != null)
            {
                _fusedLocationProviderClient.RemoveLocationUpdates(_locationCallback);
            }
        }

        private Notification BuildNotification()
        {
            var builder = new Notification.Builder(this, ChannelId)
                .SetContentTitle("StriveUp")
                .SetContentText("Tracking your location")
                .SetSmallIcon(Resource.Drawable.notification)
                .SetOngoing(true); // persistent

            return builder.Build();
        }

        private class LocationCallbackImpl : LocationCallback
        {
            public override void OnLocationResult(LocationResult result)
            {
                base.OnLocationResult(result);
                var location = result.LastLocation;
                if (location != null)
                {
                    var mauiLocation = new Location(location.Latitude, location.Longitude, location.Time)
                    {
                        Accuracy = location.HasAccuracy ? location.Accuracy : 0,
                        Altitude = location.HasAltitude ? location.Altitude : 0,
                        Speed = location.HasSpeed ? location.Speed : 0
                    };
                    LocationUpdated?.Invoke(null, mauiLocation);
                }
            }
        }

        private const string ChannelId = "location_channel";

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Location Tracking", NotificationImportance.Default)
                {
                    Description = "Tracks your location during activities"
                };
                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }
        }
    }
}