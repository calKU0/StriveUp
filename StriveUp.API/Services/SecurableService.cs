using Microsoft.Extensions.Options;
using StriveUp.API.Interfaces;
using StriveUp.Infrastructure.Data.Settings;

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