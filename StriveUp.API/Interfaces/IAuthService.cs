using Microsoft.AspNetCore.Identity;
using StriveUp.Shared.DTOs;
using System.Security.Claims;
using LoginRequest = StriveUp.Shared.DTOs.LoginRequest;
using RegisterRequest = StriveUp.Shared.DTOs.RegisterRequest;

namespace StriveUp.API.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, JwtResponse Token)> LoginAsync(LoginRequest request);

        Task<(IdentityResult Result, JwtResponse Token)> RegisterAsync(RegisterRequest request);

        Task<(bool Success, JwtResponse Token)> ExternalLoginAsync(ClaimsPrincipal externalUser);

        Task<JwtResponse> RefreshToken(RefreshTokenRequest request);
    }
}