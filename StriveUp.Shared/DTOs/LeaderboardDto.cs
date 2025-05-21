using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class LeaderboardDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public int Distance { get; set; }
        public double TotalDuration { get; set; }
        public double Speed { get; set; }
        public string MeasurmentType { get; set; }
        public string ActivityType { get; set; }
        public DateTime DateStart { get; set; }
    }
}
