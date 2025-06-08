using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class SynchroProvider
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; } = false;

        public string? IconUrl { get; set; }
    }
}