using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivitySpeedDto
    {
        [Required]
        public int UserActivityId { get; set; }
        [Required]
        public double SpeedValue { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
    }
}
