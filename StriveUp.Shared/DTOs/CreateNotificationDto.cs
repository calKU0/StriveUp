using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public string UserId { get; set; }

        public string ActorId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        public string? RedirectUrl { get; set; }
    }

}
