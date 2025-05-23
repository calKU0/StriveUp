using StriveUp.Shared.DTOs.Leaderboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class UserBestEffortsStatsDto
    {
        public string SegmentName { get; set; }
        public string SegmentShortName { get; set; }
        public double TotalDuration { get; set; }
        public double Speed { get; set; }
        public DateTime ActivityDate { get; set; }
        public int ActivityId { get; set; }
    }
}
