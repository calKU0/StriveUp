using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class Level
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LevelNumber { get; set; }

        [Required]
        public int XP { get; set; }

        [Required]
        public int TotalXP { get; set; }
    }
}