using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StriveUp.Shared.DTOs; 
using StriveUp.Sync.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace StriveUp.Sync.Application.Services
{
    public class UserSyncService : IUserSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _username;
        private readonly string _password;

        public UserSyncService(IHttpClientFactory httpClient, ILogger<UserSyncService> logger, IConfiguration config, IServiceProvider serviceProvider)
        {
            _httpClient = httpClient.CreateClient("StriveUpClient");
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                try
                {
                    IHealthDataProvider provider = user.SynchroProviderName switch
                    {
                        "Google Fit" => _serviceProvider.GetRequiredService<GoogleFitProvider>(),
                        "Fitbit" => _serviceProvider.GetRequiredService<FitbitProvider>(),
                        _ => throw new NotSupportedException("Unsupported provider")
                    };

                    var activities = await provider.GetUserActivitiesAsync(user);
                    if (activities == null || activities.Count == 0)
                    {
                        _logger.LogInformation($"No activities found for user {user.UserId}");
                        continue;
                    }

                    foreach (var activity in activities)
                    {
                        try
                        {
                            activity.UserId = user.UserId;
                            var jsonActivity = JsonSerializer.Serialize(activity);
                            var request = new HttpRequestMessage(HttpMethod.Post, "activity/addActivity")
                            {
                                Content = new StringContent(jsonActivity, Encoding.UTF8, "application/json")
                            };

                            var responseActivity = await _httpClient.SendAsync(request);

                            if (!response.IsSuccessStatusCode)
                            {
                                var error = await responseActivity.Content.ReadAsStringAsync();
                                _logger.LogWarning($"Failed to send activity for user {activity.UserId}. Status: {responseActivity.StatusCode}. Response: {error}");
                            }
                            else
                            {
                                _logger.LogInformation($"Activity sent successfully for user {activity.UserId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Exception sending activity for user {user.UserId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed syncing for user {user.UserId}");
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
