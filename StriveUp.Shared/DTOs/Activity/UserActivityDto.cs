using StriveUp.Shared.DTOs.Activity;

namespace StriveUp.Shared.DTOs
{
    public class UserActivityDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        //public List<string>? ImageUrls { get; set; }
        public double DurationSeconds { get; set; }

        public int Distance { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int CaloriesBurned { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public bool IsPublic { get; set; }
        public bool ShowHeartRate { get; set; }
        public bool ShowPace { get; set; }
        public bool ShowCalories { get; set; }
        public double? AvarageSpeed { get; set; }
        public double? MaxSpeed { get; set; }
        public int? AvarageHr { get; set; }
        public int? MaxHr { get; set; }
        public int? ElevationGain { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public List<ActivityCommentDto>? Comments { get; set; }
        public List<GeoPointDto>? Route { get; set; }
        public List<ActivityHrDto>? HrData { get; set; }
        public List<ActivitySpeedDto>? SpeedData { get; set; }
        public List<ActivityElevationDto>? ElevationData { get; set; }
        public List<ActivitySplitDto>? Splits { get; set; }
    }
}