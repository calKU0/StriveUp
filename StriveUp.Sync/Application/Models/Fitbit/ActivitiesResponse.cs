namespace StriveUp.Sync.Application.Models.Fitbit
{
    public class FitbitActivityResponse
    {
        public List<Activity> Activities { get; set; }
        // NOT USING YET

        //public ActiveZoneMinutes ActiveZoneMinutes { get; set; }
        //public Pagination Pagination { get; set; }
    }

    public class Activity
    {
        public string ActivityName { get; set; }
        public long ActivityTypeId { get; set; }
        public int? AverageHeartRate { get; set; }
        public double Distance { get; set; }
        public double? ElevationGain { get; set; }
        public List<HeartRateZone> HeartRateZones { get; set; }
        public long LogId { get; set; }
        public string LogType { get; set; }
        public long OriginalDuration { get; set; }
        public DateTime OriginalStartTime { get; set; }
        public double? Pace { get; set; }
        public double? Speed { get; set; }

        // NOT USING YET

        //public double Calories { get; set; }
        //public string CaloriesLink { get; set; }
        //public string DetailsLink { get; set; }
        //public string DistanceUnit { get; set; }
        //public long Duration { get; set; }
        //public bool? HasActiveZoneMinutes { get; set; }
        //public string HeartRateLink { get; set; }
        //public DateTime? LastModified { get; set; }
        //public ManualValuesSpecified ManualValuesSpecified { get; set; }
        //public Source Source { get; set; }
        //public DateTime StartTime { get; set; }
        //public int Steps { get; set; }
        //public string TcxLink { get; set; }
        //public List<ActivityLevel> ActivityLevel { get; set; }
    }

    public class HeartRateZone
    {
        public string Name { get; set; } // Cardio, Fat Burn, Out of Range, Peak
        public int Min { get; set; }
        public int Max { get; set; }
        public int Minutes { get; set; }
        public double CaloriesOut { get; set; }
    }

    // NOT USING YET

    //public class ManualValuesSpecified
    //{
    //    public bool Calories { get; set; }
    //    public bool Distance { get; set; }
    //    public bool Steps { get; set; }
    //}

    //public class Source
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public List<string> TrackerFeatures { get; set; } // CALORIES, DISTANCE, etc.
    //    public string Type { get; set; }
    //    public string Url { get; set; }
    //}

    //public class ActivityLevel
    //{
    //    public string Name { get; set; } // sedentary | lightly | fairly | very
    //    public int Minutes { get; set; }
    //}

    //public class ActiveZoneMinutes
    //{
    //    public int TotalMinutes { get; set; }
    //    public List<HeartRateZoneDetail> MinutesInHeartRateZones { get; set; }
    //}

    //public class HeartRateZoneDetail
    //{
    //    public string Type { get; set; } // OUT_OF_ZONE | FAT_BURN | CARDIO | PEAK
    //    public string ZoneName { get; set; } // Out of Range | Fat Burn | Cardio | Peak
    //    public int Minutes { get; set; }
    //    public double MinuteMultiplier { get; set; }
    //    public int Order { get; set; }
    //}

    //public class Pagination
    //{
    //    public string AfterDate { get; set; }
    //    public int? Limit { get; set; }
    //    public string Next { get; set; }
    //    public int? Offset { get; set; }
    //    public string Previous { get; set; }
    //    public string Sort { get; set; }
    //}

}
