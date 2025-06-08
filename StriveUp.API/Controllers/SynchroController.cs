using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StriveUp.API.Models;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Data.Settings;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using System.Security.Claims;
using System.Text.Json;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SynchroController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOptions<GoogleSettings> _googleSettings;
        private readonly IOptions<FitbitSettings> _fitbitSettings;
        private readonly IOptions<StravaSettings> _stravaSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SynchroController(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager, IOptions<GoogleSettings> googleSettings, IHttpClientFactory httpClientFactory, IOptions<FitbitSettings> fitbitSettings, IOptions<StravaSettings> stravaSettings)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _googleSettings = googleSettings;
            _httpClientFactory = httpClientFactory;
            _fitbitSettings = fitbitSettings;
            _stravaSettings = stravaSettings;
        }

        [HttpGet("availableProviders")]
        public async Task<ActionResult<List<SynchroProviderDto>>> GetAvailableProviders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var allProviders = await _context.SynchroProviders.ToListAsync();

                var connectedProviderIds = await _context.UserSynchros
                    .Where(us => us.UserId == userId)
                    .Select(us => us.SynchroId)
                    .ToListAsync();

                var availableProviders = allProviders
                    .Where(p => !connectedProviderIds.Contains(p.Id))
                    .OrderByDescending(p => p.IsActive)
                    .ToList();

                var dtos = _mapper.Map<List<SynchroProviderDto>>(availableProviders);

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("userSynchros")]
        public async Task<ActionResult<UserSynchroDto>> GetUserSynchros()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var providers = await _context.UserSynchros
                    .Include(s => s.SynchroProvider)
                    .Where(s => s.UserId == userId)
                    .ToListAsync();

                var dtos = _mapper.Map<List<UserSynchroDto>>(providers);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("allUsersSynchros")]
        public async Task<ActionResult<UserSynchroDto>> GetAllUserSynchros()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users
                    .Include(u => u.Level)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (userId == null) return Unauthorized();

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    return Forbid();
                }

                var providers = await _context.UserSynchros
                .Include(s => s.SynchroProvider)
                .ToListAsync();

                var dtos = _mapper.Map<List<UserSynchroDto>>(providers);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("updateUserSynchro/{id}")]
        public async Task<IActionResult> UpdateUserSynchro(int id, [FromBody] UpdateUserSynchroDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var userSynchro = await _context.UserSynchros
                    .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

                if (userSynchro == null)
                    return NotFound("UserSynchro not found");

                _mapper.Map(dto, userSynchro);
                userSynchro.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("deleteUserSynchro/{id}")]
        public async Task<IActionResult> DeleteUserSynchro(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var synchro = await _context.UserSynchros
                    .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

                if (synchro == null)
                    return NotFound("UserSynchro not found");

                _context.UserSynchros.Remove(synchro);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("exchange-code")]
        public async Task<IActionResult> ExchangeCode([FromBody] CodeExchangeDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Code))
                {
                    return BadRequest("Invalid request data");
                }

                HttpRequestMessage tokenRequest = new();

                if (dto.State.Contains("googlefit"))
                {
                    tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
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
                }
                else if (dto.State.Contains("fitbit"))
                {
                    tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.fitbit.com/oauth2/token")
                    {
                        Content = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "code", dto.Code },
                            { "client_id", _fitbitSettings.Value.ClientId },
                            { "client_secret", _fitbitSettings.Value.ClientSecret },
                            { "redirect_uri", _fitbitSettings.Value.RedirectUri },
                            { "grant_type", "authorization_code" }
                        })
                    };

                    var credentials = $"{_fitbitSettings.Value.ClientId}:{_fitbitSettings.Value.ClientSecret}";
                    var base64Credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
                    tokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
                }
                else if (dto.State.Contains("strava"))
                {
                    tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://www.strava.com/oauth/token")
                    {
                        Content = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "code", dto.Code },
                            { "client_id", _stravaSettings.Value.ClientId },
                            { "client_secret", _stravaSettings.Value.ClientSecret },
                            //{ "redirect_uri", _stravaSettings.Value.RedirectUri },
                            { "grant_type", "authorization_code" }
                        })
                    };

                    //var credentials = $"{_fitbitSettings.Value.ClientId}:{_fitbitSettings.Value.ClientSecret}";
                    //var base64Credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
                    //tokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
                }
                else
                {
                    Console.WriteLine("Invalid provider");
                    return BadRequest("Invalid provider");
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(tokenRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Failed to get tokens: " + content);
                }

                var tokenData = JsonSerializer.Deserialize<OAuthTokenResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                    return Unauthorized();

                int synchroId = 0;

                if (dto.State.Contains("googlefit"))
                {
                    synchroId = _context.SynchroProviders.First(p => p.Name == "Google Fit").Id;
                }
                else if (dto.State.Contains("fitbit"))
                {
                    synchroId = _context.SynchroProviders.First(p => p.Name == "Fitbit").Id;
                }
                else if (dto.State.Contains("strava"))
                {
                    synchroId = _context.SynchroProviders.First(p => p.Name == "Strava").Id;
                }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("updateTokens/{id}/{userId}")]
        public async Task<IActionResult> UpdateTokens(int id, string userId, [FromBody] UpdateTokenDto dto)
        {
            try
            {
                var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (callerId == null)
                    return Unauthorized();

                var userSynchro = await _context.UserSynchros
                    .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

                if (userSynchro == null)
                    return NotFound("UserSynchro not found");

                _mapper.Map(dto, userSynchro);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}