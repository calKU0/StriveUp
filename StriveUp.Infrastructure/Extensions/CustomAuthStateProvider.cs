using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace StriveUp.Infrastructure.Extensions
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, ICustomAuthStateProvider
    {
        private readonly ITokenStorageService _tokenStorage;
        private readonly HttpClient _httpClient;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ITokenStorageService tokenStorage, IHttpClientFactory httpClient)
        {
            _tokenStorage = tokenStorage;
            _httpClient = httpClient.CreateClient("ApiClient");
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetToken();

            if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
            {
                var newToken = await TryRefreshToken();
                token = newToken ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            else
            {
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                _currentUser = new ClaimsPrincipal(identity);
            }

            return new AuthenticationState(_currentUser);
        }


        public async Task NotifyUserAuthentication(JwtResponse jwt)
        {
            try
            {
                await _tokenStorage.StoreToken(jwt.Token);
                await _tokenStorage.StoreRefreshToken(jwt.RefreshToken);
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(jwt.Token), "jwt");
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
                await _tokenStorage.ClearRefreshToken();
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

        private async Task<string?> TryRefreshToken()
        {
            var token = await _tokenStorage.GetToken();
            var refreshToken = await _tokenStorage.GetRefreshToken();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
                return null;

            try
            {
                var request = new RefreshTokenRequest { AccessToken = token, RefreshToken = refreshToken };
                var response = await _httpClient.PostAsJsonAsync("auth/refresh-token", request);

                if (!response.IsSuccessStatusCode) return null;

                var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                if (jwt == null) return null;

                await _tokenStorage.StoreToken(jwt.Token);
                await _tokenStorage.StoreRefreshToken(jwt.RefreshToken);

                return jwt.Token;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Token refresh failed: {ex.Message}");
                return null;
            }
        }

    }
}
