using Android.App;
using Android.Content;
using Android.OS;
using StriveUp.Shared.Interfaces;
using AndroidApp = Android.App.Application;
using AndroidNet = Android.Net;
using AndroidProvider = Android.Provider;

namespace StriveUp.MAUI.Platforms.Android;

public class ActivityTrackingService : IActivityTrackingService
{
    private readonly Context _context;
    private Location? _lastKnownLocation;

    public event EventHandler<Location>? LocationUpdated;

    public ActivityTrackingService()
    {
        _context = AndroidApp.Context;

        // Subscribe to the static LocationUpdated event from the Android service
        LocationForegroundService.LocationUpdated += (sender, location) =>
        {
            _lastKnownLocation = location;
            LocationUpdated?.Invoke(this, location);
        };
    }

    public Task StartAsync(bool isIndoor = false, bool startNow = false)
    {
        var intent = new Intent(_context, typeof(LocationForegroundService));
        intent.PutExtra("isIndoor", isIndoor);
        intent.PutExtra("startNow", startNow); // Controls whether notification timer starts now

        //RequestIgnoreBatteryOptimizations();

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            _context.StartForegroundService(intent);
        else
            _context.StartService(intent);

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        var intent = new Intent(_context, typeof(LocationForegroundService));
        _context.StopService(intent);
        return Task.CompletedTask;
    }

    public Task PauseAsync()
    {
        var intent = new Intent(_context, typeof(LocationForegroundService));
        intent.PutExtra("command", "pause");
        _context.StartForegroundService(intent);
        return Task.CompletedTask;
    }

    public Task ResumeAsync(bool isIndoor = false)
    {
        var intent = new Intent(_context, typeof(LocationForegroundService));
        intent.PutExtra("command", "resume");
        intent.PutExtra("isIndoor", isIndoor);
        _context.StartForegroundService(intent);
        return Task.CompletedTask;
    }

    public Task<Location?> GetLastKnownLocationAsync()
    {
        return Task.FromResult(_lastKnownLocation);
    }

    private void RequestIgnoreBatteryOptimizations()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            var packageName = _context.PackageName;
            var pm = (PowerManager)_context.GetSystemService(Context.PowerService);
            if (!pm.IsIgnoringBatteryOptimizations(packageName))
            {
                var intent = new Intent(AndroidProvider.Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(AndroidNet.Uri.Parse("package:" + packageName));
                intent.AddFlags(ActivityFlags.NewTask);
                _context.StartActivity(intent);
            }
        }
    }

    public bool IsBackgroundActivityAllowed()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            var pm = (PowerManager)_context.GetSystemService(Context.PowerService);
            var am = (ActivityManager)_context.GetSystemService(Context.ActivityService);

            bool ignoringBatteryOptimizations = pm.IsIgnoringBatteryOptimizations(_context.PackageName);
            bool isBackgroundRestricted = Build.VERSION.SdkInt >= BuildVersionCodes.P
                && ((ActivityManager)_context.GetSystemService(Context.ActivityService))?.IsBackgroundRestricted == true;

            return ignoringBatteryOptimizations && !isBackgroundRestricted;
        }

        return true;
    }
}