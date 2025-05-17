using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StriveUp.Sync.Application.Interfaces;

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
                return new TokenResult { AccessToken = user.AccessToken };
            }

            var payload = new Dictionary<string, string>
            {
                { "client_id", _config["GoogleClientId"] },
                { "client_secret", _config["GoogleClientSecret"] },
                { "refresh_token", user.RefreshToken },
                { "grant_type", "refresh_token" }
            };

            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(payload));
            _logger.LogInformation(await response.Content.ReadAsStringAsync());
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement;

            var accessToken = data.GetProperty("access_token").GetString();
            var expiresIn = data.GetProperty("expires_in").GetInt32();

            return new TokenResult
            {
                AccessToken = accessToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn)
            };
        }
    }

    public class TokenResult
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

}
