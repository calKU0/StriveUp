﻿using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Profile;

namespace StriveUp.Shared.Interfaces
{
    public interface IProfileService
    {
        Task<(bool Success, ErrorResponse? Error, UserProfileDto profile)> GetProfile(string userName);

        Task<(bool Success, ErrorResponse? Error)> EditProfile(EditUserProfileDto profile);

        Task<SimpleUserDto> GetSimpleUserData(string userName);

        Task<UserConfigDto> GetUserConfig();

        Task<bool> UpdateUserConfig(UpdateUserConfigDto config);
    }
}