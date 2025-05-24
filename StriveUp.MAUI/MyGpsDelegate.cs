#if ANDROID

using AndroidX.Core.App;
using Resource = Microsoft.Maui.Controls.Resource;

#endif

using Microsoft.Extensions.Logging;
using Shiny;
using Shiny.Locations;
using StriveUp.Shared.Interfaces;

namespace StriveUp.MAUI
{
    public partial class MyGpsDelegate : GpsDelegate, IGpsService
    {
        public event Action<GpsReading> ReadingReceived;

        public MyGpsDelegate(ILogger<MyGpsDelegate> logger) : base(logger)
        {
            this.MinimumDistance = Distance.FromMeters(5);
            this.MinimumTime = TimeSpan.FromSeconds(3);
        }

        protected override Task OnGpsReading(GpsReading reading)
        {
            ReadingReceived?.Invoke(reading);
            return Task.CompletedTask;
        }
    }

#if ANDROID

    public partial class MyGpsDelegate : IAndroidForegroundServiceDelegate
    {
        public void Configure(NotificationCompat.Builder builder)
        {
            builder
                .SetContentTitle("StriveUp")
                .SetContentText("Tracking your location in background")
                .SetSmallIcon(Resource.Mipmap.appicon_foreground);
        }
    }

#endif
}