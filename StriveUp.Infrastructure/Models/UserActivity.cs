using StriveUp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public int DurationMinutes { get; set; }
        [Required]
        public int CaloriesBurned { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DateEnd { get; set; }
        public List<ActivityLike> ActivityLikes { get; set; } = new();
        public List<ActivityComment> ActivityComments { get; set; } = new();
        public List<GeoPoint> Route { get; set; } = new();

        //[NotMapped]
        //public List<string>? ImageUrls { get; set; }
    }
}
