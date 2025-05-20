using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public NotificationService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<List<NotificationDto>> GetMyNotificationsAsync()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.GetFromJsonAsync<List<NotificationDto>>("notifications");
                return response ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            await _httpClient.PostAsync($"notifications/readAll", null);
        }

        public async Task MarkAsReadAsync(int id)
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            await _httpClient.PostAsync($"notifications/read/{id}", null);
        }
    }

}
