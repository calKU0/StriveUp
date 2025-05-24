using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs;
using StriveUp.Sync.Application.Interfaces;
using StriveUp.Sync.Application.Models;
using System.Text.Json;

namespace StriveUp.Sync.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<TokenService> _logger;

        public TokenService(HttpClient httpClient, IConfiguration config, ILogger<TokenService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<TokenResult> GetAccessTokenAsync(UserSynchroDto user)
        {
            if (user.TokenExpiresAt > DateTime.UtcNow)
            {
                return new TokenResult
                {
                    Token = new UpdateTokenDto
                    {
                        AccessToken = user.AccessToken,
                        RefreshToken = user.RefreshToken,
                        ExpiresIn = (int)(user.TokenExpiresAt - DateTime.UtcNow).TotalSeconds
                    },
                    IsNewToken = false
                };
            }

            Dictionary<string, string> payload = new();
            string endpointUri = string.Empty;

            if (user.SynchroProviderName == "Google Fit")
            {
                endpointUri = "https://oauth2.googleapis.com/token";
                payload = new Dictionary<string, string>
                {
                    { "client_id", _config["GoogleClientId"] },
                    { "client_secret", _config["GoogleClientSecret"] },
                    { "refresh_token", user.RefreshToken },
                    { "grant_type", "refresh_token" }
                };
            }
            else if (user.SynchroProviderName == "Fitbit")
            {
                endpointUri = "https://api.fitbit.com/oauth2/token";
                payload = new Dictionary<string, string>
                {
                    { "refresh_token", user.RefreshToken },
                    { "grant_type", "refresh_token" }
                };

                var credentials = $"{_config["FitbitClientId"]}:{_config["FitbitClientSecret"]}";
                var base64Credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
            }

            var requestContent = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(endpointUri, requestContent);

            _logger.LogInformation(await response.Content.ReadAsStringAsync());

            if (!response.IsSuccessStatusCode)
                return null;

            var responseStream = await response.Content.ReadAsStreamAsync();
            var tokenData = await JsonSerializer.DeserializeAsync<UpdateTokenDto>(responseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new TokenResult
            {
                Token = tokenData,
                IsNewToken = true
            };
        }
    }
}