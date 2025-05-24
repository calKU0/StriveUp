using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Profile
{
    public class SimpleUserDto
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Avatar { get; set; }
    }
}
