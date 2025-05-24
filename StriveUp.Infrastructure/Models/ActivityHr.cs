using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class ActivityHr
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserActivityId { get; set; }

        [Required]
        public UserActivity UserActivity { get; set; }

        [Required]
        public int HearthRateValue { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }
    }
}