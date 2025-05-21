using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriveUp.API.Interfaces;
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

        [HttpGet("top-time-spent")]
        public async Task<IActionResult> GetTopTimeSpent()
        {
            var userId = GetUserId();
            var leaderboard = await _leaderboardService.GetTopTimeSpentAsync(userId);
            return Ok(leaderboard);
        }
    }
}
