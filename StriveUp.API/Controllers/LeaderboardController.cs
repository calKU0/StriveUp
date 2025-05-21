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

        [HttpGet("top-distance/{activityType}/{distance}")]
        public async Task<IActionResult> GetTopDistance(string activityType, int distance)
        {
            var userId = GetUserId();

            var leaderboard = await _leaderboardService.GetTopDistanceAsync(userId, activityType, distance);
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
