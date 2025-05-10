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

        [Required]
        public required string MeasurementType { get; set; }

        [Required]
        public required string DefaultDistanceUnit { get; set; }

        public bool UseHeartRate { get; set; }

        public bool ElevationRelevant { get; set; }

        public bool IndoorCapable { get; set; }

        // Foreign key relationship
        public int ActivityId { get; set; }
        public Activity Activity { get; set; } = null!;
    }

}
