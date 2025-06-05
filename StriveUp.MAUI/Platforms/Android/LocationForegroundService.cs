using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Timers;

namespace StriveUp.MAUI.Platforms.Android
{
    [Service(ForegroundServiceType = ForegroundService.TypeLocation)]
    public class LocationForegroundService : Service
    {
        private IFusedLocationProviderClient _fusedLocationProviderClient;
        private LocationCallback _locationCallback;
        private System.Timers.Timer _notificationTimer;
        private DateTime _startTime;
        private bool _isIndoor;
        private PowerManager.WakeLock _wakeLock;

        public static event EventHandler<Location> LocationUpdated;

        public override void OnCreate()
        {
            base.OnCreate();
            _fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            _locationCallback = new LocationCallbackImpl();
            CreateNotificationChannel();
            AcquireWakeLock();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Immediate, lightweight notification to lock foreground service
            StartForeground(1, BuildNotification("Starting location service..."));

            // WakeLock if needed
            //if (_wakeLock == null)
            //{
            //    PowerManager powerManager = (PowerManager)GetSystemService(PowerService);
            //    _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "MyApp::LocationWakeLock");
            //    _wakeLock.SetReferenceCounted(false);
            //    _wakeLock.Acquire(10 * 60 * 1000);
            //}

            // Continue with logic
            _isIndoor = intent?.GetBooleanExtra("isIndoor", false) ?? false;
            bool startNow = intent?.GetBooleanExtra("startNow", false) ?? false;
            string command = intent?.GetStringExtra("command");

            if (command == "pause")
            {
                _notificationTimer?.Stop();
                UpdateNotificationPaused();
                if (!_isIndoor)
                    StopLocationUpdates();
            }
            else if (command == "resume")
            {
                if (_notificationTimer == null)
                {
                    _startTime = DateTime.UtcNow;
                }
                StartNotificationTimer();
                UpdateLiveNotification();
                if (!_isIndoor)
                    StartLocationUpdates();
            }
            else if (startNow)
            {
                _startTime = DateTime.UtcNow;
                StartNotificationTimer();
                UpdateLiveNotification();
                if (!_isIndoor)
                    StartLocationUpdates();
            }
            else
            {
                if (!_isIndoor)
                    StartLocationUpdates();
            }

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _notificationTimer?.Stop();
            _notificationTimer?.Dispose();
            if (!_isIndoor)
            {
                StopLocationUpdates();
            }
            ReleaseWakeLock();
        }

        public override IBinder OnBind(Intent intent) => null;

        private void StartLocationUpdates()
        {
            var locationRequest = new LocationRequest.Builder(Priority.PriorityHighAccuracy, 5000)
                .SetMinUpdateIntervalMillis(2500)
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

        private Notification BuildNotification(string message, string duration = "")
        {
            string contentText = string.IsNullOrEmpty(duration)
                ? message
                : $"{message} - {duration}";

            var builder = new Notification.Builder(this, ChannelId)
                .SetContentTitle("StriveUp")
                .SetContentText(contentText)
                .SetSmallIcon(Resource.Drawable.notification)
                .SetOngoing(true)
                .SetOnlyAlertOnce(true);

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

        private string GetNotificationMessage()
        {
            return "Tracking your activity";
        }

        private void StartNotificationTimer()
        {
            _notificationTimer = new System.Timers.Timer(10000); // 10 sec
            _notificationTimer.Elapsed += (s, e) =>
            {
                var duration = DateTime.UtcNow - _startTime;
                string formatted = $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}";

                var notification = BuildNotification(GetNotificationMessage(), formatted);
                StartForeground(1, notification);
            };
            _notificationTimer.Start();
        }

        private void UpdateNotificationPaused()
        {
            var builder = new Notification.Builder(this, ChannelId)
                .SetContentTitle("StriveUp")
                .SetContentText("Tracking paused")
                .SetSmallIcon(Resource.Drawable.notification)
                .SetOngoing(true)
                .SetOnlyAlertOnce(true);

            var notification = builder.Build();
            var manager = (NotificationManager)GetSystemService(NotificationService);
            manager.Notify(1, notification);
        }

        private void AcquireWakeLock()
        {
            var powerManager = (PowerManager)GetSystemService(PowerService);
            if (_wakeLock == null)
            {
                _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "StriveUp::LocationLock");
                _wakeLock.SetReferenceCounted(false);
                _wakeLock.Acquire(); // No timeout, indefinite until released manually
            }
        }

        private void ReleaseWakeLock()
        {
            if (_wakeLock?.IsHeld ?? false)
            {
                _wakeLock.Release();
                _wakeLock = null;
            }
        }

        private void UpdateLiveNotification()
        {
            string msg = GetNotificationMessage();
            StartForeground(1, BuildNotification(msg, "00:00:00"));
        }
    }
}