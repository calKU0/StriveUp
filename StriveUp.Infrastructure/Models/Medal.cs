using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StriveUp.Infrastructure.Data;

namespace StriveUp.Infrastructure.Models
{
    public class Medal
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public double TargetValue { get; set; } 

        [Required]
        public int Level { get; set; }

        [Required]
        public string Frequency { get; set; }

        public bool IsOneTime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public int ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}
