using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using StriveUp.Shared.DTOs;
using LoginRequest = StriveUp.Shared.DTOs.LoginRequest;
using RegisterRequest = StriveUp.Shared.DTOs.RegisterRequest;

namespace StriveUp.API.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequest request, HttpContext httpContext);
        Task<IdentityResult> RegisterAsync(RegisterRequest request);
    }
}
