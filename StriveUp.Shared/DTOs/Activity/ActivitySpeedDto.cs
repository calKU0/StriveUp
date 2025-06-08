using System.ComponentModel.DataAnnotations;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivitySpeedDto
    {
        public int UserActivityId { get; set; }
        public double SpeedValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}