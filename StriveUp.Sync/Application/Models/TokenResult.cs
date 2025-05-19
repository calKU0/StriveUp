using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Sync.Application.Models
{
    public class TokenResult
    {
        public UpdateTokenDto Token { get; set; }
        public bool IsNewToken { get; set; }
    }
}
