using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StriveUp.Sync.Application.Models.Strava
{
    public class StravaActivityResponse
    {
        [JsonPropertyName("resource_state")]
        public int ResourceState { get; set; }

        [JsonPropertyName("athlete")]
        public Athlete Athlete { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance")]
        public double Distance { get; set; }

        [JsonPropertyName("moving_time")]
        public int MovingTime { get; set; }

        [JsonPropertyName("elapsed_time")]
        public int ElapsedTime { get; set; }

        [JsonPropertyName("total_elevation_gain")]
        public double TotalElevationGain { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("sport_type")]
        public string SportType { get; set; }

        [JsonPropertyName("workout_type")]
        public object WorkoutType { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("start_date_local")]
        public DateTime StartDateLocal { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("start_latlng")]
        public object StartLatlng { get; set; }

        [JsonPropertyName("end_latlng")]
        public object EndLatlng { get; set; }

        [JsonPropertyName("location_city")]
        public object LocationCity { get; set; }

        [JsonPropertyName("location_state")]
        public object LocationState { get; set; }

        [JsonPropertyName("location_country")]
        public string LocationCountry { get; set; }

        [JsonPropertyName("achievement_count")]
        public int AchievementCount { get; set; }

        [JsonPropertyName("photo_count")]
        public int PhotoCount { get; set; }

        [JsonPropertyName("map")]
        public Map Map { get; set; }

        [JsonPropertyName("trainer")]
        public bool Trainer { get; set; }

        [JsonPropertyName("commute")]
        public bool Commute { get; set; }

        [JsonPropertyName("manual")]
        public bool Manual { get; set; }

        [JsonPropertyName("private")]
        public bool Private { get; set; }

        [JsonPropertyName("gear_id")]
        public string GearId { get; set; }

        [JsonPropertyName("average_speed")]
        public double AverageSpeed { get; set; }

        [JsonPropertyName("max_speed")]
        public double MaxSpeed { get; set; }

        [JsonPropertyName("average_cadence")]
        public double AverageCadence { get; set; }

        [JsonPropertyName("average_watts")]
        public double AverageWatts { get; set; }

        [JsonPropertyName("weighted_average_watts")]
        public int WeightedAverageWatts { get; set; }

        [JsonPropertyName("kilojoules")]
        public double Kilojoules { get; set; }

        [JsonPropertyName("device_watts")]
        public bool DeviceWatts { get; set; }

        [JsonPropertyName("has_heartrate")]
        public bool HasHeartrate { get; set; }

        [JsonPropertyName("average_heartrate")]
        public double AverageHeartrate { get; set; }

        [JsonPropertyName("max_heartrate")]
        public double MaxHeartrate { get; set; }

        [JsonPropertyName("max_watts")]
        public double MaxWatts { get; set; }

        [JsonPropertyName("total_photo_count")]
        public int TotalPhotoCount { get; set; }
    }

    public class Athlete
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("resource_state")]
        public int ResourceState { get; set; }
    }

    public class Map
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("summary_polyline")]
        public object SummaryPolyline { get; set; }

        [JsonPropertyName("resource_state")]
        public int ResourceState { get; set; }
    }
}