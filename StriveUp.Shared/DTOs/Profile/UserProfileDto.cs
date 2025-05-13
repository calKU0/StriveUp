using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Profile
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public int CurrentXP { get; set; }
        public int LevelNumber { get; set; }
        public int LevelTotalXP { get; set; }
        public List<UserActivityDto> Activities { get; set; }
        public List<MedalDto> Medals { get; set; }
    }
}
