using Android.App;
using Android.Content;
using Android.OS;
using Microsoft.Maui.Devices.Sensors;
using StriveUp.Shared.Interfaces;
using AndroidApp = Android.App.Application;

namespace StriveUp.MAUI.Platforms.Android;

public class LocationTrackingService : ILocationTrackingService
{
    private readonly Context _context;
    private Location? _lastKnownLocation;

    public event EventHandler<Location>? LocationUpdated;

    public LocationTrackingService()
    {
        _context = AndroidApp.Context;

        // Subscribe to the static LocationUpdated event from the Android service
        LocationForegroundService.LocationUpdated += (sender, location) =>
        {
            _lastKnownLocation = location;
            LocationUpdated?.Invoke(this, location);
        };
    }

    public Task StartAsync()
    {
        var intent = new Intent(_context, typeof(LocationForegroundService));
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            _context.StartForegroundService(intent);
        }
        else
        {
            _context.StartService(intent);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        var intent = new Intent(_context, typeof(LocationForegroundService));
        _context.StopService(intent);
        return Task.CompletedTask;
    }

    public Task<Location?> GetLastKnownLocationAsync()
    {
        return Task.FromResult(_lastKnownLocation);
    }
}