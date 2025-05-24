using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface IMedalService
    {
        Task<List<MedalDto>> GetAllMedalsAsync();

        Task<List<MedalDto>> GetUserMedalsAsync();

        Task<int> GetMedalsToClaimCountAsync();

        Task<bool> ClaimMedal(int medalId);
    }
}