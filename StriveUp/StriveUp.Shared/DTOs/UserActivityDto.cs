using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class UserActivityDto
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        //public List<string>? ImageUrls { get; set; }
        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int DurationMinutes { get; set; }
    }
}
