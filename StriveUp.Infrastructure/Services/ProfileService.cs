using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Profile;
using StriveUp.Shared.Interfaces;
using System.Net.Http.Json;

namespace StriveUp.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public ProfileService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _tokenStorage = tokenStorage;
        }

        public async Task<(bool Success, ErrorResponse? Error, UserProfileDto profile)> GetProfile(string userName)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.GetFromJsonAsync<UserProfileDto>($"profile/{userName}");

                if (response != null)
                {
                    return (true, null, response);
                }

                return (false, new ErrorResponse
                {
                    Message = "Failed to load user profile."
                }, null);
            }
            catch (Exception ex)
            {
                return (false, new ErrorResponse
                {
                    Message = "An error occurred while retrieving the profile.",
                    Details = ex.Message
                }, null);
            }
        }

        public async Task<(bool Success, ErrorResponse? Error)> EditProfile(EditUserProfileDto profile)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var response = await _httpClient.PutAsJsonAsync("profile", profile);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, errorResponse);
            }
            catch (Exception ex)
            {
                return (false, new ErrorResponse
                {
                    Message = "An error occurred while updating the profile.",
                    Details = ex.Message
                });
            }
        }

        public async Task<SimpleUserDto> GetSimpleUserData(string userName)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                return await _httpClient.GetFromJsonAsync<SimpleUserDto>($"profile/simpleData/{userName}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserConfigDto> GetUserConfig()
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                return await _httpClient.GetFromJsonAsync<UserConfigDto>($"profile/config") ?? new();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserConfig(UpdateUserConfigDto config)
        {
            try
            {
                await _httpClient.AddAuthHeaderAsync(_tokenStorage);
                var result = await _httpClient.PatchAsJsonAsync($"profile/config", config);
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}