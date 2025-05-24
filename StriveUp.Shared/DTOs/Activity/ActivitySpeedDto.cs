using System.ComponentModel.DataAnnotations;

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