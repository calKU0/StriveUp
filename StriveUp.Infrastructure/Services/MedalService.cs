using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class MedalService : IMedalsService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public MedalService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public Task<bool> AwardMedalToUserAsync(string userId, int medalId)
        {
            throw new NotImplementedException();
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
