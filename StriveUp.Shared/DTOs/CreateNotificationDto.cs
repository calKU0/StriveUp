using System.ComponentModel.DataAnnotations;

namespace StriveUp.Shared.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public string UserId { get; set; }

        public string ActorId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(256)]
        public string Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        public string? RedirectUrl { get; set; }
    }
}