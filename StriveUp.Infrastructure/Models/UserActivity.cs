using StriveUp.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StriveUp.Infrastructure.Models
{
    public class UserActivity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public AppUser User { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [Required]
        [JsonIgnore]
        public Activity Activity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 1440)]
        public double DurationSeconds { get; set; }

        public int Distance { get; set; }

        [Required]
        public int CaloriesBurned { get; set; }

        public double? AvarageSpeed { get; set; }
        public double? MaxSpeed { get; set; }
        public int? AvarageHr { get; set; }
        public int? MaxHr { get; set; }
        public int? ElevationGain { get; set; }

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateEnd { get; set; }

        [Required]
        public bool IsManuallyAdded { get; set; } = false;

        [Required]
        public bool isSynchronized { get; set; } = false;

        public string? SynchroId { get; set; }
        public List<ActivityLike>? ActivityLikes { get; set; }
        public List<ActivityComment>? ActivityComments { get; set; }
        public List<GeoPoint>? Route { get; set; }
        public List<ActivityHr>? HrData { get; set; }
        public List<ActivitySpeed>? SpeedData { get; set; }
        public List<ActivityElevation>? ElevationData { get; set; }

        //[NotMapped]
        //public List<string>? ImageUrls { get; set; }
    }
}