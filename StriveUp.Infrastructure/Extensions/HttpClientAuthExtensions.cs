using StriveUp.Shared.Interfaces;
using System.Net.Http.Headers;

namespace StriveUp.Infrastructure.Extensions
{
    public static class HttpClientAuthExtensions
    {
        public static async Task AddAuthHeaderAsync(this HttpClient client, ITokenStorageService tokenStorage)
        {
            var token = await tokenStorage.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                if (client.DefaultRequestHeaders.Contains("Authorization"))
                {
                    client.DefaultRequestHeaders.Remove("Authorization");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}