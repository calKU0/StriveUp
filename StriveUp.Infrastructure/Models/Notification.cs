using StriveUp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string? ActorId { get; set; }
        public AppUser? Actor { get; set; }

        [Required]
        [MaxLength(256)]
        public string Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        public string? RedirectUrl { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
