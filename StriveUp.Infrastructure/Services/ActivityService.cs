using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class ActivityService : IActivityService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public ActivityService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<List<ActivityDto>> GetAvailableActivitiesAsync()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.GetFromJsonAsync<List<ActivityDto>>("activity/availableActivities") ?? new List<ActivityDto>();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<ActivityDto>();
            }
        }

        public async Task<List<ActivityDto>?> GetActivitiesWithSegments()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.GetFromJsonAsync<List<ActivityDto>>("activity/activities-with-segments") ?? new List<ActivityDto>();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<ActivityDto>();
            }
        }

        public async Task<UserActivityDto> GetActivityByIdAsync(int activityId)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                return await _httpClient.GetFromJsonAsync<UserActivityDto>($"activity/{activityId}") ?? new UserActivityDto();
            }
            catch
            {
                return new UserActivityDto();
            }
        }

        public async Task<ActivityDto> GetActivityConfig(int id)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                return await _httpClient.GetFromJsonAsync<ActivityDto>($"activity/activityConfig/{id}") ?? new ActivityDto();
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
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.PostAsJsonAsync("activity/addActivity", activity);
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

        public async Task<List<UserActivityDto>> GetFeedAsync(int page, int pageSize)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);

                var url = $"activity/feed?page={page}&pageSize={pageSize}";
                return await _httpClient.GetFromJsonAsync<List<UserActivityDto>>(url) ?? new List<UserActivityDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching paginated feed: {ex}");
                return new List<UserActivityDto>();
            }
        }

        public async Task<List<UserActivityDto>?> GetUserActivitiesAsync(string userName, int page, int pageSize)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);

                var url = $"activity/userActivities?userName={userName}&page={page}&pageSize={pageSize}";
                return await _httpClient.GetFromJsonAsync<List<UserActivityDto>>(url) ?? new List<UserActivityDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching paginated feed: {ex}");
                return new List<UserActivityDto>();
            }
        }

        public async Task LikeActivityAsync(int activityId)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
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
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var payload = new { Content = content };
                await _httpClient.PostAsJsonAsync($"activity/comment/{activityId}", payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<List<ActivityCommentDto>?> GetActivityComments(int activityId)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                return await _httpClient.GetFromJsonAsync<List<ActivityCommentDto>?>($"activity/activityComments/{activityId}") ?? new List<ActivityCommentDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<ActivityCommentDto>();
            }
        }

        public async Task<bool> DeleteUserActivityAsync(int activityId)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.DeleteAsync($"activity/{activityId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public async Task<bool> UpdateUserActivityAsync(int activityId, UpdateUserActivityDto activity)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.PatchAsJsonAsync($"activity/{activityId}", activity);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}