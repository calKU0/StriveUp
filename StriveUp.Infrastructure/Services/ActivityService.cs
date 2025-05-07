using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Shared.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace StriveUp.Infrastructure.Services
{
    public class ActivityService : IActivityService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public ActivityService(HttpClient httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
        }

        public async Task<List<ActivityDto>?> GetAvailableActivitiesAsync()
        {
            try
            {
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var result = await _httpClient.GetFromJsonAsync<List<ActivityDto>>("activity/availableActivities");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<UserActivityDto?> GetActivityByIdAsync(int activityId)
        {
            try
            {
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await _httpClient.GetFromJsonAsync<UserActivityDto>($"activity/{activityId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddActivityAsync(CreateUserActivityDto activity)
        {
            try
            {
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("activity/AddUserActivity", activity);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> UpdateActivityAsync(int activityId, CreateUserActivityDto activity)
        {
            try
            {
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PutAsJsonAsync($"activity/{activityId}", activity);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<UserActivityDto>?> GetUserActivitiesAsync()
        {
            try
            {
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await _httpClient.GetFromJsonAsync<List<UserActivityDto>>("activity/userFeed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public async Task LikeActivityAsync(int activityId)
        {
            try
            {
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                await _httpClient.PostAsync($"activity/like/{activityId}", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task AddCommentAsync(int activityId, string content)
        {
            try
            {
                var payload = new { Content = content };
                await _httpClient.PostAsJsonAsync($"activity/comment/{activityId}", payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}