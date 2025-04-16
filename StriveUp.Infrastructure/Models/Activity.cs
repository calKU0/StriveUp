using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class Activity
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        [Required]
        public required int AverageCaloriesPerHour { get; set; }

        public ICollection<UserActivity>? UserActivities { get; set; }
    }
}
