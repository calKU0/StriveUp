using StriveUp.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class ActivityLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserActivityId { get; set; }

        public UserActivity UserActivity { get; set; }

        [Required]
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}