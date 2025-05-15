using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class CreateUserSynchroDto
    {
        public int SynchroProviderId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
