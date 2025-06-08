namespace StriveUp.Sync.Application.Models.Strava
{
    public class StravaStreamSet
    {
        public StravaStream<List<int>> Time { get; set; }
        public StravaStream<List<List<double>>> LatLng { get; set; }
        public StravaStream<List<int>> Heartrate { get; set; }
        public StravaStream<List<float>> VelocitySmooth { get; set; }
        public StravaStream<List<float>> Altitude { get; set; }
    }

    public class StravaStream<T>
    {
        public string Type { get; set; }
        public string SeriesType { get; set; }
        public string OriginalSize { get; set; }
        public T Data { get; set; }
    }
}