using Microsoft.AspNetCore.Identity;
using StriveUp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriveUp.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Url] public string Avatar { get; set; } = "images/icons/user.png";

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "XP cannot be negative")]
        public int CurrentXP { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int LevelId { get; set; } = 1;

        [Required]
        public Level Level { get; set; }

        [MaxLength(150)]
        public string? Bio { get; set; }

        [Range(0, 1000, ErrorMessage = "Weight must be a realistic value")]
        public double? Weight { get; set; }

        [Range(0, 300, ErrorMessage = "Height must be a realistic value")]
        public double? Height { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = "Not Specified";
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Links
        public ICollection<UserActivity>? UserActivities { get; set; }
        public ICollection<MedalEarned>? MedalsEarned { get; set; }
        public ICollection<UserFollower>? Followers { get; set; }
        public ICollection<UserFollower>? Following { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<UserSynchro> UserSynchros { get; set; }
    }
}
