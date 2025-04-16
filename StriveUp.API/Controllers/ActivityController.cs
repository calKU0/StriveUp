using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserActivitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserActivitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserActivity(UserActivityDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activity = await _context.Activities.FindAsync(dto.ActivityId);
            if (activity == null)
            {
                return BadRequest("Invalid activity ID.");
            }

            var calories = Convert.ToInt32(Math.Round((decimal)(activity.AverageCaloriesPerHour / 60) * dto.DurationMinutes));

            var userActivity = new UserActivity
            {
                UserId = userId!,
                Name = activity.Name,
                ActivityId = dto.ActivityId,
                Description = dto.Description,
                //ImageUrls = dto.ImageUrls ?? new List<string>(),
                DurationMinutes = dto.DurationMinutes,
                CaloriesBurned = calories
            };

            _context.UserActivities.Add(userActivity);
            await _context.SaveChangesAsync();

            return Ok(userActivity);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserActivities()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activities = await _context.UserActivities
                .Include(ua => ua.Activity)
                .Where(ua => ua.UserId == userId)
                .ToListAsync();

            return Ok(activities);
        }
    }
}
