using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IMedalsService
    {
        Task<List<MedalDto>> GetAllMedalsAsync();
        Task<List<MedalDto>> GetUserMedalsAsync();
        Task<bool> AwardMedalToUserAsync(string userId, int medalId);
    }
}
