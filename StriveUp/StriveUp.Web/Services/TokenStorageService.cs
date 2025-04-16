using Blazored.LocalStorage;
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

        public Task StoreToken(string token) => _localStorage.SetItemAsync(TokenKey, token).AsTask();
        public Task<string?> GetToken() => _localStorage.GetItemAsync<string>(TokenKey).AsTask();
        public Task ClearToken() => _localStorage.RemoveItemAsync(TokenKey).AsTask();
    }
}
