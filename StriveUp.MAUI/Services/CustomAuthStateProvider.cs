using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using StriveUp.Shared.Interfaces;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace StriveUp.MAUI.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, ICustomAuthStateProvider
    {
        private readonly ITokenStorageService _tokenStorage;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private readonly HttpClient _httpClient;

        public CustomAuthStateProvider(ITokenStorageService tokenStorage, HttpClient httpClient)
        {
            _tokenStorage = tokenStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _tokenStorage.GetToken();

                if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
                {
                    // If there's no token or the token is expired, reset state to unauthenticated
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
                else
                {
                    // If token is valid, set authorization header and extract claims
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _currentUser = new ClaimsPrincipal(identity);
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


        // Helper method to parse claims from JWT
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }

        // Helper method to check if a JWT token is expired
        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == "exp")?.Value;

            if (string.IsNullOrWhiteSpace(expClaim))
                return true;

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
            return expirationTime <= DateTime.UtcNow;
        }
    }
}
