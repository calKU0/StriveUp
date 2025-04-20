using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string FriendId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
