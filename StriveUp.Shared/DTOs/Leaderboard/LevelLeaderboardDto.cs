namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class LevelLeaderboardDto
    {
        public string UserId { get; set; } = "";
        public string Username { get; set; } = "";
        public string UserAvatar { get; set; }
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
    }
}