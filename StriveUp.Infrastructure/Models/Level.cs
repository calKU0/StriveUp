using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
