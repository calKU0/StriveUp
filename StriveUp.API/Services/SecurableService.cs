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
using StriveUp.API.Interfaces;

namespace StriveUp.API.Services
{
    public class SecurableService : ISecurableService
    {
        private readonly IOptions<MapboxSettings> _config;
        public SecurableService(IOptions<MapboxSettings> config)
        {
            _config = config;
        }
        public async Task<string> GetMapboxTokenAsync()
        {
            return _config.Value.AccessToken;
        }
    }
}
