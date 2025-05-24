using StriveUp.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriveUp.Infrastructure.Models
{
    public class UserSynchro
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        public int SynchroId { get; set; }

        [Required]
        [ForeignKey("SynchroId")]
        public SynchroProvider SynchroProvider { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}