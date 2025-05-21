using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class SegmentConfig
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string ShortName { get; set; } = null!;
        [Required]
        public int DistanceMeters { get; set; }
        [Required]
        public int ActivityId { get; set; }
        public Activity Activity { get; set; } = null!;
    }
}
