using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardDto>> GetTopDistanceAsync(string activityType, int distance);
        Task<List<LeaderboardDto>> GetTopTimeSpentAsync();
    }
}
