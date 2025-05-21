using StriveUp.Shared.DTOs;

namespace StriveUp.API.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardDto>> GetBestFollowersEfforts(string userId, int activityId, int distance);
        Task<List<LeaderboardDto>> GetTopTimeSpentAsync(string userId);
    }

}
