using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IMedalService
    {
        Task<List<MedalDto>> GetAllMedalsAsync();
        Task<List<MedalDto>> GetUserMedalsAsync();
        Task<bool> ClaimMedal(int medalId);
    }
}
