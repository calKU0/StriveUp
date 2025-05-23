using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class UserStatsResponseDto
    {
        public List<UserBestEffortsStatsDto> BestEfforts { get; set; }
        public UserActivityStatsDto ActivityStats { get; set; }
    }
}
