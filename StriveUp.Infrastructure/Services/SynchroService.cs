using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Services
{
    public class SynchroService : ISynchroService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public SynchroService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<List<UserSynchroDto>> GetAvailableProvidersAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<UserSynchroDto>>("synchro/availableProviders");
        }

        public async Task<List<UserSynchroDto>> GetUserSynchrosAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetFromJsonAsync<List<UserSynchroDto>>("synchro/userSynchros");
        }

        public async Task<HttpResponseMessage> AddUserSynchroAsync(CreateUserSynchroDto dto)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.PostAsJsonAsync("synchro/addUserSynchro", dto);
        }

        public async Task<HttpResponseMessage> UpdateUserSynchroAsync(int id, UpdateUserSynchroDto dto)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.PutAsJsonAsync($"synchro/updateUserSynchro/{id}", dto);
        }

        public async Task<HttpResponseMessage> DeleteUserSynchroAsync(int id)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.DeleteAsync($"synchro/deleteUserSynchro/{id}");
        }
    }
}
