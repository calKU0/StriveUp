using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StriveUp.Infrastructure.Data.Settings;

namespace StriveUp.Infrastructure.Services
{
    public class SynchroService : ISynchroService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;
        private readonly IOptions<GoogleSettings> _googleSettings;

        public SynchroService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage, IOptions<GoogleSettings> googleSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
            _googleSettings = googleSettings;
        }

        public async Task<List<UserSynchroDto>> GetAvailableProvidersAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<UserSynchroDto>>("synchro/availableProviders");
        }

        public async Task<List<UserSynchroDto>> GetUserSynchrosAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<UserSynchroDto>>("synchro/userSynchros");
        }

        public async Task<HttpResponseMessage> UpdateUserSynchroAsync(int id, UpdateUserSynchroDto dto)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.PutAsJsonAsync($"synchro/updateUserSynchro/{id}", dto);
        }

        public async Task<HttpResponseMessage> DeleteUserSynchroAsync(int id)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.DeleteAsync($"synchro/deleteUserSynchro/{id}");
        }
        public string GetGoogleFitOAuthUrl()
        {
            var scopes = new[]
            {
                "https://www.googleapis.com/auth/fitness.activity.read",
                "https://www.googleapis.com/auth/fitness.location.read",
                "https://www.googleapis.com/auth/fitness.heart_rate.read"
            };

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                          $"?client_id={Uri.EscapeDataString(_googleSettings.Value.ClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_googleSettings.Value.RedirectUri)}" +
                          $"&response_type=code" +
                          $"&scope={Uri.EscapeDataString(string.Join(" ", scopes))}" +
                          $"&access_type=offline" +
                          $"&prompt=consent";

            return authUrl;
        }

        public async Task<HttpResponseMessage> ExchangeGoogleFitCodeAsync(string code)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var payload = new { Code = code };
            return await _httpClient.PostAsJsonAsync("googlefit/exchange-code", payload);
        }

    }
}
