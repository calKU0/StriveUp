namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class LeaderboardDto
    {
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public string UserAvatar { get; set; }
        public int Distance { get; set; }
        public double TotalDuration { get; set; }
        public double Speed { get; set; }
        public DateTime ActivityDate { get; set; }
        public int ActivityId { get; set; }
    }
}