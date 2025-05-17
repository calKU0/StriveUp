using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using StriveUp.Infrastructure.Data.Settings;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/googlefit")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class GoogleFitOAuthController : ControllerBase
    {
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleFitOAuthController(IConfiguration configuration, AppDbContext context, IHttpClientFactory httpClientFactory, IOptions<GoogleSettings> googleSettings)
        {
            _googleSettings = googleSettings;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("exchange-code")]
        public async Task<IActionResult> ExchangeCode([FromBody] CodeExchangeDto dto)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "code", dto.Code },
                    { "client_id", _googleSettings.Value.ClientId },
                    { "client_secret", _googleSettings.Value.ClientSecret },
                    { "redirect_uri", _googleSettings.Value.RedirectUri },
                    { "grant_type", "authorization_code" }
                })
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(tokenRequest);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to get tokens: " + content);
            }

            var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var synchroId = _context.SynchroProviders.First(p => p.Name == "Google Fit").Id;

            var userSynchro = new UserSynchro
            {
                UserId = userId,
                SynchroId = synchroId,
                IsActive = true,
                AccessToken = tokenData.AccessToken,
                RefreshToken = tokenData.RefreshToken,
                TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn)
            };

            _context.UserSynchros.Add(userSynchro);
            await _context.SaveChangesAsync();

            return Ok();
        }


        public class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }

    }

}
