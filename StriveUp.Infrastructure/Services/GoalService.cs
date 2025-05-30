using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.Components;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Profile;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Services
{
    public class GoalService : IGoalService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public GoalService(IHttpClientFactory httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<bool> AddGoalAsync(CreateUserGoalDto goal)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.PostAsJsonAsync($"goals", goal);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteGoalAsync(int goalId)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.DeleteAsync($"goals/{goalId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserGoalDto>> GetGoalsAsync()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.GetFromJsonAsync<List<UserGoalDto>>("goals");
                return response ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}