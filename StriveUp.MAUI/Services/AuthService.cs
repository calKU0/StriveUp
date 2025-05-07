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

        public async Task<(bool Success, string? ErrorMessage)> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("auth/login", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return (false, errorResponse?.Message ?? "Login failed.");
                }

                var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                if (jwt == null) return (false, "Invalid response from server.");

                await _authStateProvider.NotifyUserAuthentication(jwt.Token);

                return (true, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, $"Unexpected error: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            _http.DefaultRequestHeaders.Authorization = null;
            await _authStateProvider.NotifyUserLogout();
        }

        public async Task<(bool Success, List<string>? Errors)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("auth/register", request);
                if (response.IsSuccessStatusCode)
                {
                    var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                    if (jwt != null)
                    {
                        await _authStateProvider.NotifyUserAuthentication(jwt.Token);
                        return (true, null);
                    }

                    return (false, new List<string> { "Unknown error occurred." });
                }
                else
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return (false, errorResponse?.Errors ?? new List<string> { "Registration failed." });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, new List<string> { $"Unexpected error: {ex.Message}" });
            }
        }

        private class ErrorResponse
        {
            public string? Message { get; set; }
            public List<string>? Errors { get; set; }
        }
    }
}
