using StriveUp.Shared.DTOs;

namespace StriveUp.API.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardDto>> GetTopDistanceAsync(string userId, string activityType, int distance);
        Task<List<LeaderboardDto>> GetTopTimeSpentAsync(string userId);
    }

}
