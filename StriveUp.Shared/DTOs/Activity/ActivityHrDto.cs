using System.ComponentModel.DataAnnotations;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivityHrDto
    {
        [Required]
        public int UserActivityId { get; set; }

        [Required]
        public int HearthRateValue { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }
    }
}