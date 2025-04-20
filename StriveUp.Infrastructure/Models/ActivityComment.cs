using StriveUp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class ActivityComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserActivityId { get; set; }
        public UserActivity UserActivity { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
