using Microsoft.Extensions.Options;
using StriveUp.Infrastructure.Data.Settings;
using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class SynchroService : ISynchroService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IOptions<FitbitSettings> _fitbitSettings;
        private readonly IOptions<StravaSettings> _stravaSettings;

        public SynchroService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage, IOptions<GoogleSettings> googleSettings, IOptions<FitbitSettings> fitbitSettings, IOptions<StravaSettings> stravaSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
            _googleSettings = googleSettings;
            _fitbitSettings = fitbitSettings;
            _stravaSettings = stravaSettings;
        }

        public async Task<List<SynchroProviderDto>> GetAvailableProvidersAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<SynchroProviderDto>>("synchro/availableProviders");
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

        public string GetOAuthUrl(string provider)
        {
            string authUrl = string.Empty;

            if (provider == "googlefit")
            {
                var scopes = new[]
                {
                    "https://www.googleapis.com/auth/fitness.activity.read",
                    "https://www.googleapis.com/auth/fitness.location.read",
                    "https://www.googleapis.com/auth/fitness.heart_rate.read"
                };

                authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                           $"?client_id={Uri.EscapeDataString(_googleSettings.Value.ClientId)}" +
                           $"&redirect_uri={Uri.EscapeDataString(_googleSettings.Value.RedirectUri)}" +
                           $"&response_type=code" +
                           $"&state=googlefit" +
                           $"&scope={Uri.EscapeDataString(string.Join(" ", scopes))}" +
                           $"&access_type=offline" +
                           $"&prompt=consent";
            }
            else if (provider == "fitbit")
            {
                var scopes = new[]
                {
                    "activity",
                    "heartrate",
                    "location"
                };

                authUrl = $"https://www.fitbit.com/oauth2/authorize" +
                          $"?client_id={Uri.EscapeDataString(_fitbitSettings.Value.ClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_fitbitSettings.Value.RedirectUri)}" +
                          $"&response_type=code" +
                          $"&state=fitbit" +
                          $"&scope={Uri.EscapeDataString(string.Join(" ", scopes))}" +
                          $"&prompt=consent";
            }
            else if (provider == "strava")
            {
                var scopes = new[]
                {
                    "activity:read"
                };

                authUrl = $"https://www.strava.com/oauth/mobile/authorize" +
                          $"?client_id={Uri.EscapeDataString(_stravaSettings.Value.ClientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(_stravaSettings.Value.RedirectUri)}" +
                          $"&response_type=code" +
                          $"&state=strava" +
                          $"&approval_prompt=auto" +
                          $"&scope={Uri.EscapeDataString(string.Join(" ", scopes))}";
            }

            return authUrl;
        }

        public async Task<HttpResponseMessage> ExchangeCodeAsync(string code, string state)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var payload = new { Code = code, State = state };
            return await _httpClient.PostAsJsonAsync("synchro/exchange-code", payload);
        }
    }
}