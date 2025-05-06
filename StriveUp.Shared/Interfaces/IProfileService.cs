using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetUserProfile(AppUser user);
        Task<UserProfileDto> UpdateUserProfile(AppUser user);
    }
}
