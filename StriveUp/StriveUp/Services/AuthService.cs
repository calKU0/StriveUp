using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StriveUp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient http, CustomAuthStateProvider authStateProvider)
        {
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode) return false;

            var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
            if (jwt == null) return false;

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Token);
            await _authStateProvider.NotifyUserAuthentication(jwt.Token);

            return true;
        }

        public async Task LogoutAsync()
        {
            _http.DefaultRequestHeaders.Authorization = null;
            await _authStateProvider.NotifyUserLogout();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
