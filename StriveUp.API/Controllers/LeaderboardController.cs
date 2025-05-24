using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriveUp.API.Interfaces;
using StriveUp.Shared.DTOs.Leaderboard;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardService _leaderboardService;

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("best-followers-efforts/{activityId}/{distance}")]
        public async Task<IActionResult> GetBestFollowersEfforts(int activityId, int distance)
        {
            var userId = GetUserId();

            var leaderboard = await _leaderboardService.GetBestFollowersEfforts(userId, activityId, distance);
            return Ok(leaderboard);
        }

        [HttpGet("followers-distance/{activityId}/{timeframe}")]
        public async Task<IActionResult> GetFollowersDistanceLeaderboard(int activityId, string timeframe)
        {
            var userId = GetUserId();

            var leaderboard = await _leaderboardService.GetFollowersDistanceLeaderboard(userId, activityId, timeframe.ToLower());
            return Ok(leaderboard);
        }

        [HttpGet("followers-level")]
        public async Task<IActionResult> GetFollowersLevel()
        {
            var userId = GetUserId();

            var levelsDto = await _leaderboardService.GetFollowersLevelLeaderboard(userId);
            return Ok(levelsDto);
        }

        [HttpGet("user-stats/{userName}/{activityId}")]
        public async Task<IActionResult> GetUserStats(string userName, int activityId)
        {
            var (bestEfforts, activityStats) = await _leaderboardService.GetUserStats(userName, activityId);
            var response = new UserStatsResponseDto
            {
                BestEfforts = bestEfforts,
                ActivityStats = activityStats
            };
            return Ok(response);
        }
    }
}