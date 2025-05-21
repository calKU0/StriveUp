using GeolocatorPlugin.Abstractions;
using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace StriveUp.Infrastructure.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;
        public LeaderboardService(ITokenStorageService tokenStorage, IHttpClientFactory httpClient)
        {
            _tokenStorage = tokenStorage;
            _httpClient = httpClient.CreateClient("ApiClient");
        }
        public async Task<List<LeaderboardDto>> GetTopDistanceAsync(string activityType, int distance)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<LeaderboardDto>>($"leaderboard/top-distance/{activityType}/{distance}");
            return result ?? new();
        }

        public async Task<List<LeaderboardDto>> GetTopTimeSpentAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<LeaderboardDto>>("leaderboard/top-time-spent");
            return result ?? new();
        }
    }
}
