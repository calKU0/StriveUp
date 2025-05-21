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
        Task<List<LeaderboardDto>> GetBestFollowersEfforts(SegmentDto SelectedSegment);
        Task<List<LeaderboardDto>> GetTopTimeSpentAsync();
    }
}
