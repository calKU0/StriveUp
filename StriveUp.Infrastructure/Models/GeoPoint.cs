using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class GeoPoint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserActivityId { get; set; }

        [Required]
        public UserActivity UserActivity { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}