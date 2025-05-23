using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardDto>> GetBestFollowersEfforts(SegmentDto SelectedSegment);
        Task<UserStatsResponseDto> GetUserStats(string userId, int activityId);
        Task<List<DistanceLeaderboardDto>> GetFollowersDistanceLeaderboard(int activityId, string Timeframe);
        Task<List<LevelLeaderboardDto>> GetFollowersLevelLeaderboard();
    }
}
