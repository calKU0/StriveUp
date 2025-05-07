using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Profile
{
    public class UserProfileDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public List<UserActivityDto> Activities { get; set; }
        public List<MedalDto> Medals { get; set; }
    }
}
