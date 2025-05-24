using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class ActivitySplit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserActivityId { get; set; }

        [ForeignKey("UserActivityId")]
        public UserActivity UserActivity { get; set; }

        [Required]
        public int SplitNumber { get; set; }

        [Required]
        public double DistanceMeters { get; set; }

        public double? AvgSpeed { get; set; }

        public int? AvgHr { get; set; }

        public int? ElevationGain { get; set; }
    }
}