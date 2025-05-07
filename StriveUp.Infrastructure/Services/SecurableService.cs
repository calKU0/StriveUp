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

namespace StriveUp.Infrastructure.Services
{
    public class SecurableService : ISecurableService
    {
        private readonly HttpClient _httpClient;
        public SecurableService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("ApiClient");
        }
        public async Task<string> GetMapboxTokenAsync()
        {
            return await _httpClient.GetStringAsync("securable/mapboxToken");
        }
    }
}
