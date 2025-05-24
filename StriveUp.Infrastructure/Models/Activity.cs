using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class Activity
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? Icon { get; set; }

        [Required]
        public int AverageCaloriesPerHour { get; set; }

        public ActivityConfig? Config { get; set; }
        public ICollection<UserActivity>? UserActivities { get; set; }
        public ICollection<Medal>? Medals { get; set; }
        public ICollection<SegmentConfig>? SegmentConfigs { get; set; }
    }
}