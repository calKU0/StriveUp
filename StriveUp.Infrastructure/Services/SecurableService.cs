using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.Interfaces;

namespace StriveUp.Infrastructure.Services
{
    public class SecurableService : ISecurableService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public SecurableService(IHttpClientFactory httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<string> GetMapboxTokenAsync()
        {
            await _httpClient.AddAuthHeaderAsync(_tokenStorage);
            return await _httpClient.GetStringAsync("securable/mapboxToken");
        }
    }
}