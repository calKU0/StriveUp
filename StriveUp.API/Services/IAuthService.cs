using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using StriveUp.Shared.DTOs;
using LoginRequest = StriveUp.Shared.DTOs.LoginRequest;
using RegisterRequest = StriveUp.Shared.DTOs.RegisterRequest;

namespace StriveUp.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Token)> LoginAsync(LoginRequest request);
        Task<(IdentityResult Result, string Token)> RegisterAsync(RegisterRequest request);
    }
}
