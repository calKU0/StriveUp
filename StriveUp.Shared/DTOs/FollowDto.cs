using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class FollowDto
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Avatar { get; set; }
        public bool IsFollowed { get; set; }
    }

}
