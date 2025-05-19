using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class ActivityConfig
    {
        [Key]
        public int Id { get; set; }

        public required string MeasurementType { get; set; }

        [Required]
        public required string DefaultDistanceUnit { get; set; } = "km";

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
