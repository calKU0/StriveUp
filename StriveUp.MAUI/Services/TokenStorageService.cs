using StriveUp.Shared.Interfaces;

namespace StriveUp.MAUI.Services
{
    public class TokenStorageService : ITokenStorageService
    {
        private const string TokenKey = "authToken";
        private const string RefreshTokenKey = "refreshToken";

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

        public Task StoreRefreshToken(string token)
        {
            Preferences.Set(RefreshTokenKey, token);
            return Task.CompletedTask;
        }

        public Task<string?> GetRefreshToken()
        {
            var token = Preferences.Get(RefreshTokenKey, null);
            return Task.FromResult(token);
        }

        public Task ClearRefreshToken()
        {
            Preferences.Remove(RefreshTokenKey);
            return Task.CompletedTask;
        }
    }
}