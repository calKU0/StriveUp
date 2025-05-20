using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Profile;
using StriveUp.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IProfileService
    {
        Task<(bool Success, ErrorResponse? Error, UserProfileDto profile)> GetProfile(string userName);
        Task<(bool Success, ErrorResponse? Error)> EditProfile(EditUserProfileDto profile);
        Task<SimpleUserDto> GetSimpleUserData(string userId);
    }
}
