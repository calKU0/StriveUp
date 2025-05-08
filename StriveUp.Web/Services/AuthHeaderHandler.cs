using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Web.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ITokenStorageService _tokenStorage;

        public AuthHeaderHandler(ITokenStorageService tokenStorage)
        {
            _tokenStorage = tokenStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _tokenStorage.GetToken();
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding auth header: {ex.Message}");
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
