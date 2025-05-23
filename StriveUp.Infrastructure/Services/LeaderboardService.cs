using GeolocatorPlugin.Abstractions;
using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Leaderboard;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<List<LeaderboardDto>> GetBestFollowersEfforts(SegmentDto segmentDto)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<LeaderboardDto>>($"leaderboard/best-followers-efforts/{segmentDto.ActivityId}/{segmentDto.DistanceMeters}");
            return result ?? new();
        }

        public async Task<List<DistanceLeaderboardDto>> GetFollowersDistanceLeaderboard(int activityId, string Timeframe)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<DistanceLeaderboardDto>>($"leaderboard/followers-distance/{activityId}/{Timeframe}");
            return result ?? new();
        }

        public async Task<List<LevelLeaderboardDto>> GetFollowersLevelLeaderboard()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<LevelLeaderboardDto>>($"leaderboard/followers-level");
            return result ?? new();
        }
    }
}
