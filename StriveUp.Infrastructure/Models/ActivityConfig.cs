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

        public bool UseHeartRate { get; set; } = false;

        public bool ElevationRelevant { get; set; } = false;

        public bool SpeedRelevant { get; set; } = false;

        public bool Indoor { get; set; } = false;

        [Required]
        public double PointsPerMinute { get; set; }

        // Foreign key relationship
        public int ActivityId { get; set; }
        public Activity Activity { get; set; } = null!;
    }

}
