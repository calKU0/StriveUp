using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

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
