using StriveUp.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriveUp.Infrastructure.Models
{
    public class MedalEarned
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedalId { get; set; }

        [Required]
        public Medal Medal { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [Required]
        public Activity Activity { get; set; }

        [Required]
        public DateTime DateEarned { get; set; }
    }
}