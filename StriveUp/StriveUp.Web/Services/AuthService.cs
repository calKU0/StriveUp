using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Headers;

namespace StriveUp.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ITokenStorageService _tokenStorage;

        public AuthService(HttpClient http, ITokenStorageService tokenStorage)
        {
            _http = http;
            _tokenStorage = tokenStorage;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode) return false;

            var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
            if (jwt == null) return false;

            await _tokenStorage.StoreToken(jwt.Token);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Token);
            return true;
        }

        public async Task LogoutAsync()
        {
            await _tokenStorage.ClearToken();
            _http.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }

}
