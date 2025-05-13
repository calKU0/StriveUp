using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.API.Services;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Infrastructure.Services;
using StriveUp.Shared.DTOs;
using System.Diagnostics;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MedalController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILevelService _levelService;

        public MedalController(AppDbContext context, IMapper mapper, ILevelService levelService)
        {
            _context = context;
            _mapper = mapper;
            _levelService = levelService;
        }

        [HttpGet("medals")]
        public async Task<ActionResult<IEnumerable<MedalDto>>> GetAllMedals()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activities = await _context.UserActivities
                    .Where(ua => ua.UserId == userId).ToListAsync();

                var medals = await _context.Medals
                    .ProjectTo<MedalDto>(_mapper.ConfigurationProvider) 
                    .ToListAsync();

                foreach (var medal in medals)
                {
                    var (progress, distanceToEarn) = CalculateMedalProgressAndDistance(medal, activities);
                    medal.ProgressPercent = progress;
                    medal.DistanceToEarn = distanceToEarn;
                }

                return Ok(medals);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("userMedals")]
        public async Task<ActionResult<IEnumerable<MedalDto>>> GetUserMedals()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var medals = await _context.MedalsEarned
                    .Where(m => m.UserId == userId)
                    .Include(m => m.Medal)
                    .ToListAsync();

                var medalDtos = _mapper.Map<List<MedalDto>>(medals);

                return Ok(medalDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("medalsToClaimCount")]
        public async Task<ActionResult<int>> GetMedalsToClaimCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activities = await _context.UserActivities
                    .Where(ua => ua.UserId == userId)
                    .ToListAsync();

                var earnedMedals = await _context.MedalsEarned
                    .Where(me => me.UserId == userId)
                    .ToListAsync();

                var medals = await _context.Medals
                    .ProjectTo<MedalDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                int count = 0;
                foreach (var medal in medals)
                {
                    var (progress, _) = CalculateMedalProgressAndDistance(medal, activities);
                    if (progress < 100) continue;

                    bool alreadyClaimed = false;

                    if (medal.Frequency == "Once")
                    {
                        alreadyClaimed = earnedMedals.Any(me => me.MedalId == medal.Id);
                    }
                    else if (medal.Frequency == "Weekly")
                    {
                        var today = DateTime.Today;
                        var diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                        var startOfWeek = today.AddDays(-diff);
                        var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                        alreadyClaimed = earnedMedals.Any(me =>
                            me.MedalId == medal.Id &&
                            me.DateEarned >= startOfWeek &&
                            me.DateEarned <= endOfWeek
                        );
                    }

                    if (!alreadyClaimed)
                        count++;
                }

                return Ok(count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("claim/{id:int}")]
        public async Task<ActionResult<MedalDto>> ClaimMedal(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var medal = await _context.Medals
                    .AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new { m.Id, m.IsOneTime, m.ActivityId, m.Points })
                    .FirstOrDefaultAsync();

                if (medal == null)
                    return NotFound("Medal not found.");

                if (medal.IsOneTime)
                {
                    var alreadyClaimed = await _context.MedalsEarned
                        .AsNoTracking()
                        .AnyAsync(me => me.UserId == userId && me.MedalId == medal.Id);

                    if (alreadyClaimed)
                        return BadRequest("User has already claimed this one-time medal.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);


                if (user == null)
                    return NotFound("User not found.");

                var medalEarned = new MedalEarned
                {
                    MedalId = medal.Id,
                    UserId = userId,
                    ActivityId = medal.ActivityId,
                    DateEarned = DateTime.UtcNow
                };

                _context.MedalsEarned.Add(medalEarned);

                int xpReward = medal.Points > 0 ? medal.Points : 50;
                user.CurrentXP += xpReward;

                await _levelService.UpdateUserLevelAsync(user);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        private (int ProgressPercent, double DistanceToEarn) CalculateMedalProgressAndDistance(MedalDto medal, List<UserActivity> activities)
        {
            if (medal.ActivityId == 0 || medal.TargetValue <= 0)
                return (0, medal.TargetValue);

            double totalDistance = 0;

            switch (medal.Frequency)
            {
                case "Weekly":
                    var today = DateTime.Today;
                    var diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                    var startOfWeek = today.AddDays(-diff);
                    var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                    totalDistance = activities
                        .Where(a => a.ActivityId == medal.ActivityId && a.DateStart >= startOfWeek && a.DateStart <= endOfWeek)
                        .Sum(a => a.Distance);
                    break;

                case "Once":
                    totalDistance = activities
                        .Where(a => a.ActivityId == medal.ActivityId)
                        .Sum(a => a.Distance);
                    break;

                case "Monthly":
                    // Monthly logic later
                    totalDistance = 0;
                    break;

                default:
                    return (0, medal.TargetValue);
            }

            double remaining = Math.Max(0, medal.TargetValue - totalDistance);
            int progress = (int)Math.Round((totalDistance / medal.TargetValue) * 100);
            progress = Math.Min(progress, 100);

            return (progress, remaining);
        }

    }
}
