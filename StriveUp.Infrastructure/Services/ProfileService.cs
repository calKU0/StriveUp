using StriveUp.Shared.DTOs.Profile;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;
using StriveUp.Infrastructure.Extensions;

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
                var response = await _httpClient.GetFromJsonAsync<UserProfileDto>($"user/profile/{userName}");

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
                var response = await _httpClient.PutAsJsonAsync("user/profile", profile);

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
    }
}
