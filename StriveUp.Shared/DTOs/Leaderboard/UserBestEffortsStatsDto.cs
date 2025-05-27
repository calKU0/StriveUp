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
        public int? SegmentRank { get; set; }
    }
}