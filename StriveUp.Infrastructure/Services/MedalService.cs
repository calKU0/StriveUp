using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class MedalService : IMedalService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public MedalService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<bool> ClaimMedal(int medalId)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.PostAsync($"medal/claim/{medalId}", null);
                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<List<MedalDto>> GetAllMedalsAsync()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.GetFromJsonAsync<List<MedalDto>>("medal/medals");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<int> GetMedalsToClaimCountAsync()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.GetFromJsonAsync<int>("medal/medalsToClaimCount");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public async Task<List<MedalDto>> GetUserMedalsAsync()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.GetFromJsonAsync<List<MedalDto>>("medal/userMedals");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}