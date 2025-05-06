using StriveUp.Infrastructure.Services;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StriveUp.MAUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ICustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient http, ICustomAuthStateProvider authStateProvider)
        {
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("auth/login", request);
                Debug.WriteLine(response);
                if (!response.IsSuccessStatusCode) return false;

                var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                if (jwt == null) return false;

                await _authStateProvider.NotifyUserAuthentication(jwt.Token);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            _http.DefaultRequestHeaders.Authorization = null;
            await _authStateProvider.NotifyUserLogout();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
