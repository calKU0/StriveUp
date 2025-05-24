namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class DistanceLeaderboardDto
    {
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public string UserAvatar { get; set; }
        public double TotalDistance { get; set; }
    }
}