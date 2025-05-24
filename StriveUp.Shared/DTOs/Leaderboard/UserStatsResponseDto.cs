namespace StriveUp.Shared.DTOs.Leaderboard
{
    public class UserStatsResponseDto
    {
        public List<UserBestEffortsStatsDto> BestEfforts { get; set; }
        public UserActivityStatsDto ActivityStats { get; set; }
    }
}