using StriveUp.Infrastructure.Identity;

namespace StriveUp.API.Interfaces
{
    public interface ILevelService
    {
        Task UpdateUserLevelAsync(AppUser user);
    }
}