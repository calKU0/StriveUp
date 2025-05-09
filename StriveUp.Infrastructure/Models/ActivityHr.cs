using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
