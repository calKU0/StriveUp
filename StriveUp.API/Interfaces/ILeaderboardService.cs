using StriveUp.Shared.DTOs.Leaderboard;

namespace StriveUp.API.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardDto>> GetBestFollowersEfforts(string userId, int activityId, int distance);

        Task<List<DistanceLeaderboardDto>> GetFollowersDistanceLeaderboard(string userId, int activityId, string timeframe);

        Task<List<LevelLeaderboardDto>> GetFollowersLevelLeaderboard(string userId);

        Task<(List<UserBestEffortsStatsDto>, UserActivityStatsDto)> GetUserStats(string userName, int activityId);
    }
}