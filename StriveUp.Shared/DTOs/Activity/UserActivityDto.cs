using StriveUp.Shared.DTOs.Activity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class UserActivityDto
    {
        public int Id { get; set; }
        [Required]
        public int ActivityId { get; set; }
        public string? ActivityName { get; set; }
        [Required]
        public string Title { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        //public List<string>? ImageUrls { get; set; }
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        [Required]
        public double DurationSeconds { get; set; }
        [Required]
        public int Distance { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DateEnd { get; set; }
        public int CaloriesBurned { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public List<ActivityCommentDto> Comments { get; set; } = new();
        public List<GeoPointDto>? Route { get; set; } = new();
        public List<ActivityHrDto> HrData { get; set; } = new();
    }
}
