using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Activity
{
    public class SimpleUserActivityDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public double DurationSeconds { get; set; }
        public int Distance { get; set; }
        public double? AvarageSpeed { get; set; }
        public int? ElevationGain { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int? CaloriesBurned { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public bool IsPrivate { get; set; }
        public bool ShowSpeed { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool HasNewRecord { get; set; }
        public List<ActivityCommentDto>? Comments { get; set; }
        public List<GeoPointDto>? Route { get; set; }
    }
}