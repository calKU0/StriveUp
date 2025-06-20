﻿using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Diagnostics;
using System.Net.Http.Json;

namespace StriveUp.MAUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ICustomAuthStateProvider _authStateProvider;
        private readonly ITokenStorageService _tokenStorage;

        public event Func<string, string, Task>? TokensReceived;

        public AuthService(IHttpClientFactory httpClientFactory, ICustomAuthStateProvider authStateProvider, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _authStateProvider = authStateProvider;
            _tokenStorage = tokenStorage;
        }

        public async Task<(bool Success, string? ErrorMessage)> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    return (false, errorResponse?.Message ?? "Login failed.");
                }

                var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                if (jwt == null) return (false, "Invalid response from server.");

                await _authStateProvider.NotifyUserAuthentication(jwt);

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
            await _authStateProvider.NotifyUserLogout();
        }

        public async Task DeleteAccountAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var response = await _httpClient.DeleteAsync("auth/delete");
            response.EnsureSuccessStatusCode();
            await _authStateProvider.NotifyUserLogout();
        }

        public async Task<(bool Success, List<string>? Errors)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/register", request);
                if (response.IsSuccessStatusCode)
                {
                    var jwt = await response.Content.ReadFromJsonAsync<JwtResponse>();
                    if (jwt != null)
                    {
                        await _authStateProvider.NotifyUserAuthentication(jwt);
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

        public async Task<(bool Success, string? ErrorMessage)> ExternalLoginAsync(JwtResponse jwt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwt.Token))
                    return (false, "No token provided.");

                await _authStateProvider.NotifyUserAuthentication(jwt);
                return (true, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, $"Unexpected error: {ex.Message}");
            }
        }

        public async Task StartNativeGoogleLoginAsync()
        {
            var authUrl = "https://striveupapi-emaee9awang6g4ht.polandcentral-01.azurewebsites.net/api/auth/google-login?returnUrl=striveup://login-callback";
            var callbackUrl = new Uri("striveup://login-callback");

            try
            {
                var result = await WebAuthenticator.Default.AuthenticateAsync(new Uri(authUrl), callbackUrl);

                if (result.Properties.TryGetValue("access_token", out var accessToken) &&
                    result.Properties.TryGetValue("refresh_token", out var refreshToken))
                {
                    if (TokensReceived != null)
                        await TokensReceived.Invoke(accessToken!, refreshToken!);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OAuth error: {ex.Message}");
            }
        }

        private class ErrorResponse
        {
            public string? Message { get; set; }
            public List<string>? Errors { get; set; }
        }
    }
}