using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Profile;
using StriveUp.Shared.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class FollowService : IFollowService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public FollowService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<List<UserFollowDto>> SearchUsersAsync(string keyword)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<UserFollowDto>>($"follow/search?keyword={keyword}");
            return result ?? new();
        }

        public async Task<bool> FollowAsync(string followedId)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.PostAsync($"follow/{followedId}", null);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UnfollowAsync(string followedId)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.DeleteAsync($"follow/{followedId}");
            return result.IsSuccessStatusCode;
        }

        public async Task<List<UserFollowDto>> GetUserFollowers(string userName)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<UserFollowDto>>($"follow/followers/{userName}") ?? new();
        }

        public async Task<List<UserFollowDto>> GetUserFollowing(string userName)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<UserFollowDto>>($"follow/following/{userName}") ?? new();
        }
    }
}