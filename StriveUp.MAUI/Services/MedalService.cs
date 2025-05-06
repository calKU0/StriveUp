using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StriveUp.MAUI.Services
{
    public class MedalService : IMedalsService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public MedalService(HttpClient httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient;
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
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

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
                string? token = await _tokenStorage.GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

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
