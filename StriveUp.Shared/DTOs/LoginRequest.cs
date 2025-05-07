using System.ComponentModel.DataAnnotations;

namespace StriveUp.Shared.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
