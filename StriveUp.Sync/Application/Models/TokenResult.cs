using StriveUp.Shared.DTOs;

namespace StriveUp.Sync.Application.Models
{
    public class TokenResult
    {
        public UpdateTokenDto Token { get; set; }
        public bool IsNewToken { get; set; }
    }
}