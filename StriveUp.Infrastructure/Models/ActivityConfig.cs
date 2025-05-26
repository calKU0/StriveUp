using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class ActivityConfig
    {
        [Key]
        public int Id { get; set; }

        public string? MeasurementType { get; set; }

        public string? DefaultDistanceUnit { get; set; }

        public bool UseHeartRate { get; set; } = true;

        public bool ElevationRelevant { get; set; } = true;

        public bool SpeedRelevant { get; set; } = true;

        public bool DistanceRelevant { get; set; } = true;

        public bool Indoor { get; set; } = false;

        [Required]
        public double PointsPerMinute { get; set; }

        // Foreign key relationship
        public int ActivityId { get; set; }

        public Activity Activity { get; set; } = null!;
    }
}