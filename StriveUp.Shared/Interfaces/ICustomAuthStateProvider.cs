using Microsoft.AspNetCore.Components.Authorization;
using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface ICustomAuthStateProvider
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();

        Task NotifyUserAuthentication(JwtResponse jwt);

        Task NotifyUserLogout();
    }
}