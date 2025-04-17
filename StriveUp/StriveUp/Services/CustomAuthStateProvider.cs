using Microsoft.AspNetCore.Components.Authorization;
using StriveUp.Shared.Interfaces;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StriveUp.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, ICustomAuthStateProvider
    {
        private readonly ITokenStorageService _tokenStorage;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ITokenStorageService tokenStorage)
        {
            _tokenStorage = tokenStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _tokenStorage.GetToken();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _currentUser = new ClaimsPrincipal(identity);
                }
                else
                {
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch
            {
                // SecureStorage not available yet? Use empty identity
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task NotifyUserAuthentication(string token)
        {
            try
            {
                await _tokenStorage.StoreToken(token);
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                _currentUser = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
            catch
            {
                // SecureStorage not available yet? Use empty identity
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        public async Task NotifyUserLogout()
        {
            try
            {
                await _tokenStorage.ClearToken();
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            }
            catch
            {
                // SecureStorage not available yet? Use empty identity
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
    }
}
