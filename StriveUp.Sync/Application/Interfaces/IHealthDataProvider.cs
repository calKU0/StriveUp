using StriveUp.Shared.DTOs;

namespace StriveUp.Sync.Application.Interfaces;

public interface IHealthDataProvider
{
    Task<List<CreateUserActivityDto>> GetUserActivitiesAsync(UserSynchroDto userSynchro);
}
