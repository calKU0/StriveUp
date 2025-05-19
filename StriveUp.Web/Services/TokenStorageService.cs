using Blazored.LocalStorage;
using Microsoft.JSInterop;
using StriveUp.Shared.Interfaces;

namespace StriveUp.Web.Services
{
    public class TokenStorageService : ITokenStorageService
    {
        private const string TokenKey = "authToken";
        private const string RefreshTokenKey = "refreshToken";
        private readonly ILocalStorageService _localStorage;

        public TokenStorageService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task StoreToken(string token)
        {
            await _localStorage.SetItemAsync(TokenKey, token);
        }

        public async Task<string?> GetToken()
        {
            string token = string.Empty;
            try
            {
                token = await _localStorage.GetItemAsync<string>(TokenKey);
            }
            catch (InvalidOperationException)
            {
            }
            return token;
        }

        public async Task ClearToken()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
        }

        public async Task StoreRefreshToken(string token)
        {
            try
            {
                await _localStorage.SetItemAsync(RefreshTokenKey, token);
            }
            catch (InvalidOperationException)
            {
            }
        }

        public async Task<string?> GetRefreshToken()
        {
            try
            {
                return await _localStorage.GetItemAsync<string>(RefreshTokenKey);
            }
            catch (InvalidOperationException)
            {

            }
            return string.Empty;
        }
        public async Task ClearRefreshToken() => await _localStorage.RemoveItemAsync(RefreshTokenKey);
    }
}
