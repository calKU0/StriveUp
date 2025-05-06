using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.MAUI.Services
{
    public class TokenStorageService : ITokenStorageService
    {
        private const string TokenKey = "authToken";

        public Task StoreToken(string token)
        {
            Preferences.Set(TokenKey, token);
            return Task.CompletedTask;
        }

        public Task<string?> GetToken()
        {
            var token = Preferences.Get(TokenKey, null);
            return Task.FromResult(token);
        }

        public Task ClearToken()
        {
            Preferences.Remove(TokenKey);
            return Task.CompletedTask;
        }
    }
}
