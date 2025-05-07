using Microsoft.AspNetCore.Identity;
using StriveUp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? Avatar { get; set; }
        //[MaxLength(150)]
        //public string? Bio { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserActivity>? UserActivities { get; set; }

        public ICollection<MedalEarned>? MedalsEarned { get; set; }
    }
}
