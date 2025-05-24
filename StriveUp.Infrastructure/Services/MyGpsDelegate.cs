using Shiny.Locations;

namespace StriveUp.Infrastructure.Services
{
    public class MyGpsDelegate : Shiny.Locations.IGpsDelegate
    {
        public event Action<GpsReading> ReadingReceived;

        public MyGpsDelegate()
        {
        }

        Task IGpsDelegate.OnReading(GpsReading reading)
        {
            ReadingReceived?.Invoke(reading);
            return Task.CompletedTask;
        }

        //public void Configure(NotificationCompat.Builder builder)
        //{
        //    builder
        //        .SetContentTitle("StriveUp")
        //        .SetContentText("Tracking your location in background")
        //        .SetSmallIcon(Resource.Mipmap.appicon);
        //}
    }
}