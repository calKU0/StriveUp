using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<FollowDto>> SearchUsersAsync(string keyword)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            var result = await _httpClient.GetFromJsonAsync<List<FollowDto>>($"follow/search?keyword={keyword}");
            return result ?? new();
        }

        public async Task FollowAsync(string followerId, string followedId)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            await _httpClient.PostAsync($"follow/{followedId}/follow", null);
        }

        public async Task UnfollowAsync(string followerId, string followedId)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            await _httpClient.DeleteAsync($"follow/{followedId}/unfollow");
        }
    }

}
