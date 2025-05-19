using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<(bool Success, List<string>? Errors)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string? ErrorMessage)> ExternalLoginAsync(string token);

    }
}
