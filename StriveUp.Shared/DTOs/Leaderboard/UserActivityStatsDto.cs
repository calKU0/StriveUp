using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class UserActivityStatsDto
    {
        public double TotalDistance { get; set; }
        public double TotalTime { get; set; }
        public double TotalActivities { get; set; }
        public int TotalElevationGain { get; set; }
        public double AvgWeeklyDistance { get; set; }
        public double AvgWeeklyTime { get; set; }
        public double AvgWeeklyActivities { get; set; }
        public int AvgWeeklyElevationGain { get; set; }
        public double CurrentYearDistance { get; set; }
        public double CurrentYearTime { get; set; }
        public double CurrentYearActivities { get; set; }
        public int CurrentYearElevationGain { get; set; }
    }
}
