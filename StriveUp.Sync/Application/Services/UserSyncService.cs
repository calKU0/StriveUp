using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs; 
using StriveUp.Sync.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StriveUp.Sync.Application.Services
{
    public class UserSyncService : IUserSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserSyncService> _logger;
        private readonly string _username;
        private readonly string _password;

        public UserSyncService(HttpClient httpClient, ILogger<UserSyncService> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _username = config["ApiUsername"];
            _password = config["ApiPassword"];
        }

        public async Task SyncUsersAsync()
        {
            // 1. Authenticate
            var token = await GetJwtTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Authentication failed: no token received.");
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Get userSynchros
            var response = await _httpClient.GetAsync("synchro/allUsersSynchros");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to fetch userSynchros: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var userSynchros = JsonSerializer.Deserialize<List<UserSynchroDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _logger.LogInformation($"Fetched {userSynchros.Count} userSynchros");

            // 3. Logic
            foreach (var user in userSynchros)
            {
                // perform some logic here
                var activity = new CreateUserActivityDto
                {
                    UserId = user.UserId,
                    Title = "Synchro Activity",
                    Description = "This is a synchronized activity.",
                    DateStart = DateTime.Now,
                    DateEnd = DateTime.Now + TimeSpan.FromHours(1),
                    DurationSeconds = 3600,
                    Distance = 10.0,
                    ActivityId = 4,
                    isSynchronized = true,
                    SynchronizedId = 1+DateTime.Now.Minute,
                };

                // 4. Save activity
                var activityJson = new StringContent(JsonSerializer.Serialize(activity), Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync("activity/addActivity", activityJson);

                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to add activity for user {user.UserId}: {result.StatusCode}");
                    var errorContent = await result.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Error response: {errorContent}");
                }
                else
                {
                    _logger.LogInformation($"Activity added for user {user.UserId}");
                }
            }

            _logger.LogInformation("User sync completed.");
        }

        private async Task<string> GetJwtTokenAsync()
        {
            var loginPayload = new
            {
                username = _username,
                password = _password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("auth/login", content);

            if (!response.IsSuccessStatusCode) return null;

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            return doc.RootElement.GetProperty("token").GetString();
        }
    }
}
