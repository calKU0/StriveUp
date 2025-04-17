using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using StriveUp.Shared.Interfaces;

namespace StriveUp.Web.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, ICustomAuthStateProvider
    {
        private readonly ITokenStorageService _tokenStorage;
        private readonly HttpClient _httpClient;

        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ITokenStorageService tokenStorage, HttpClient httpClient)
        {
            _tokenStorage = tokenStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _tokenStorage.GetToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _currentUser = new ClaimsPrincipal(identity);
                }
            }
            catch (InvalidOperationException)
            {
                // We're in prerendering, skip JS interop (Blazored.LocalStorage)
            }

            return new AuthenticationState(new ClaimsPrincipal(_currentUser));
        }

        public async Task NotifyUserAuthentication(string token)
        {
            await _tokenStorage.StoreToken(token);
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            _currentUser = new ClaimsPrincipal(identity);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task NotifyUserLogout()
        {
            await _tokenStorage.ClearToken();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
    }
}
