using StriveUp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class BestEffort
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        [Required]
        public int ActivityId { get; set; }
        public UserActivity UserActivity { get; set; }

        [Required]
        public int SegmentConfigId { get; set; }

        public SegmentConfig SegmentConfig { get; set; }

        [Required]
        public double DurationSeconds { get; set; }

        [Required]
        public DateTime ActivityDate { get; set; }

        public double Speed => SegmentConfig.DistanceMeters / DurationSeconds;

        public UserActivity Activity { get; set; }
    }


}
