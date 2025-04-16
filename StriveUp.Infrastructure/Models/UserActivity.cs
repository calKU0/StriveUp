using StriveUp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class UserActivity
    {
        [Key] 
        [Required] 
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public AppUser User { get; set; }
        [Required]
        public int ActivityId { get; set; }
        [Required]
        public Activity Activity { get; set; }
        [Required] 
        public string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        [Range(1, 1440)]
        public int DurationMinutes { get; set; }
        [Required]
        public int CaloriesBurned { get; set; }
        //[NotMapped]
        //public List<string>? ImageUrls { get; set; }
    }
}
