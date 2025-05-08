using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StriveUp.Shared.DTOs;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using StriveUp.Infrastructure.Data.Settings;
using StriveUp.Infrastructure.Extensions;

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
