using System.ComponentModel.DataAnnotations;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivityHrDto
    {
        public int UserActivityId { get; set; }
        public int HearthRateValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}