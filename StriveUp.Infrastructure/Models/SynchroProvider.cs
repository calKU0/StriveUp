using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class SynchroProvider
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? IconUrl { get; set; }
    }
}
