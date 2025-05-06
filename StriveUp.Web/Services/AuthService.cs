using Microsoft.AspNetCore.Components.Authorization;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;

namespace StriveUp.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ICustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ICustomAuthStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);
            if (!response.IsSuccessStatusCode) return false;

            var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
            if (jwt == null) return false;

            await _authStateProvider.NotifyUserAuthentication(jwt.Token);
            return true;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("auth/logout", null);
            await _authStateProvider.NotifyUserLogout();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }

}
