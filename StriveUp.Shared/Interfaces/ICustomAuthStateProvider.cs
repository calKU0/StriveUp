using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
