using System.Xml.Serialization;

namespace StriveUp.Sync.Application.Models.Fitbit
{
    [XmlRoot("TrainingCenterDatabase", Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public class ActivityTcxResponse
    {
        [XmlArray("Activities")]
        [XmlArrayItem("Activity")]
        public List<ActivityTcx> Activities { get; set; }
    }

    public class ActivityTcx
    {
        [XmlAttribute("Sport")]
        public string Sport { get; set; }

        [XmlElement("Id")]
        public DateTime Id { get; set; }

        [XmlElement("Lap")]
        public List<Lap> Laps { get; set; }
    }

    public class Lap
    {
        [XmlAttribute("StartTime")]
        public DateTime StartTime { get; set; }

        [XmlElement("TotalTimeSeconds")]
        public double TotalTimeSeconds { get; set; }

        [XmlElement("DistanceMeters")]
        public double DistanceMeters { get; set; }

        [XmlElement("Calories")]
        public int Calories { get; set; }

        [XmlElement("Intensity")]
        public string Intensity { get; set; }

        [XmlElement("TriggerMethod")]
        public string TriggerMethod { get; set; }

        [XmlElement("Track")]
        public Track Track { get; set; }
    }

    public class Track
    {
        [XmlElement("Trackpoint")]
        public List<Trackpoint> Trackpoints { get; set; }
    }

    public class Trackpoint
    {
        [XmlElement("Time")]
        public DateTime Time { get; set; }

        [XmlElement("Position")]
        public Position Position { get; set; }

        [XmlElement("AltitudeMeters")]
        public double AltitudeMeters { get; set; }

        [XmlElement("DistanceMeters")]
        public double DistanceMeters { get; set; }

        [XmlElement("HeartRateBpm")]
        public HeartRate HeartRateBpm { get; set; }
    }

    public class Position
    {
        [XmlElement("LatitudeDegrees")]
        public double LatitudeDegrees { get; set; }

        [XmlElement("LongitudeDegrees")]
        public double LongitudeDegrees { get; set; }
    }

    public class HeartRate
    {
        [XmlElement("Value")]
        public int Value { get; set; }
    }
}