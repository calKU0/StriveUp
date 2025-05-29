using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs.Profile;
using StriveUp.Shared.Enums;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class GoalsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GoalsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/UserGoals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGoalDto>>> GetUserGoals()
        {
            var now = DateTime.UtcNow;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userGoals = await _context.UserGoals
                .Include(a => a.Activity)
                .Where(g => g.UserId == userId).ToListAsync();

            var userActivities = await _context.UserActivities
                .Select(a => new
                {
                    a.UserId,
                    a.ActivityId,
                    a.DateStart,
                    a.Distance,
                    a.DurationSeconds
                })
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var goalDtos = _mapper.Map<List<UserGoalDto>>(userGoals);

            foreach (var dto in goalDtos)
            {
                var goal = userGoals.First(g => g.Id == dto.Id);

                var startDate = goal.Timeframe switch
                {
                    GoalTimeframe.Weekly => now.Date.AddDays(-(int)now.DayOfWeek + (now.DayOfWeek == DayOfWeek.Sunday ? -6 : 1)),
                    GoalTimeframe.Monthly => new DateTime(now.Year, now.Month, 1),
                    GoalTimeframe.Yearly => new DateTime(now.Year, 1, 1),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var relevantActivities = userActivities.Where(a =>
                    a.UserId == goal.UserId &&
                    a.ActivityId == goal.ActivityId &&
                    a.DateStart >= startDate);

                var progress = goal.Type switch
                {
                    GoalType.Activities => relevantActivities.Count(),
                    GoalType.Distance => relevantActivities.Sum(a => a.Distance),
                    GoalType.Duration => relevantActivities.Sum(a => a.DurationSeconds / 60.0),
                    _ => 0
                };

                dto.AmountCompleted = progress;
                dto.AmountRemaining = Math.Max(0, goal.TargetValue - progress);
                dto.PercentCompleted = goal.TargetValue > 0
                    ? Math.Min(100, Math.Round(progress / goal.TargetValue * 100, 1))
                    : 0;
            }

            return Ok(goalDtos);
        }

        // POST: api/UserGoals
        [HttpPost]
        public async Task<ActionResult<UserGoal>> AddUserGoal(CreateUserGoalDto userGoalDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userGoal = _mapper.Map<UserGoal>(userGoalDto);
            userGoal.UserId = userId;
            if (userGoal.Type == GoalType.Duration)
            {
                userGoal.TargetValue = userGoal.TargetValue * 60;
            }
            _context.UserGoals.Add(userGoal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserGoals), new { id = userGoal.Id }, userGoal);
        }

        // DELETE: api/UserGoals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserGoal(int id)
        {
            var userGoal = await _context.UserGoals.FindAsync(id);
            if (userGoal == null)
            {
                return NotFound();
            }

            _context.UserGoals.Remove(userGoal);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}