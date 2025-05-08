using Blazored.LocalStorage;
using Microsoft.JSInterop;
using StriveUp.Shared.Interfaces;

namespace StriveUp.Web.Services
{
    public class TokenStorageService : ITokenStorageService
    {
        private const string TokenKey = "authToken";
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
            Console.WriteLine("token:" + token);
            return token;
        }

        public async Task ClearToken()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
        }
    }
}
