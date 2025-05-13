using StriveUp.Infrastructure.Identity;

namespace StriveUp.API.Services
{
    public interface ILevelService
    {
        Task UpdateUserLevelAsync(AppUser user);
    }
}
