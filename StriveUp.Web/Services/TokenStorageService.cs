using Blazored.LocalStorage;
using Microsoft.JSInterop;
using StriveUp.Shared.Interfaces;

namespace StriveUp.Web.Services
{
    public class TokenStorageService : ITokenStorageService
    {
        private const string TokenKey = "authToken";
        private readonly ILocalStorageService _localStorage; 
        private readonly IJSRuntime _jsRuntime;

        public TokenStorageService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
        }

        public async Task StoreToken(string token)
        {
            await _localStorage.SetItemAsync(TokenKey, token);
        }

        public async Task<string?> GetToken()
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        public async Task ClearToken()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
        }
    }
}
