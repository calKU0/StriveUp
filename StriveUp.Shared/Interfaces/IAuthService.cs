using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> LoginAsync(LoginRequest request);

        Task LogoutAsync();

        Task<(bool Success, List<string>? Errors)> RegisterAsync(RegisterRequest request);

        Task<(bool Success, string? ErrorMessage)> ExternalLoginAsync(JwtResponse jwt);
    }
}