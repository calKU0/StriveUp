using StriveUp.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class UserFollower
    {
        public string FollowerId { get; set; } = default!;
        public AppUser Follower { get; set; } = default!;

        public string FollowedId { get; set; } = default!;
        public AppUser Followed { get; set; } = default!;

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}
