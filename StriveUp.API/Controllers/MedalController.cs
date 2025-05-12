using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Infrastructure.Services;
using StriveUp.Shared.DTOs;
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

        public MedalController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                    medal.ProgressPercent = CalculateMedalProgress(medal, activities);
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

        [HttpPost("claim/{id:int}")]
        public async Task<ActionResult<MedalDto>> ClaimMedal(int id)
        {
            try
            {
                // Get the medal from the database
                var medal = await _context.Medals
                                    .Include(m => m.Activity)
                                    .FirstOrDefaultAsync(m => m.Id == id);

                if (medal == null)
                {
                    return NotFound("Medal not found.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Now, we check if the medal is already awarded (one-time medals should not be awarded again)
                if (medal.IsOneTime)
                {
                    var oneTimeMedalEarned = await _context.MedalsEarned.AnyAsync(me => me.UserId == userId && me.MedalId == medal.Id);

                    if (oneTimeMedalEarned)
                    {
                        return BadRequest("User has already claimed this one-time medal.");
                    }
                }

                // Create the MedalEarned record
                var medalEarned = new MedalEarned
                {
                    MedalId = medal.Id,
                    UserId = userId,
                    ActivityId = medal.ActivityId,
                    DateEarned = DateTime.UtcNow
                };

                // Save the MedalEarned record
                _context.MedalsEarned.Add(medalEarned);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        private int CalculateMedalProgress(MedalDto medal, List<UserActivity> activities)
        {
            if (medal.ActivityId == 0 || medal.TargetValue <= 0)
                return 0;

            double totalDistance = 0;

            switch (medal.Frequency)
            {
                case "Weekly":
                    // Weekly progress calculation
                    {
                        var today = DateTime.Today;
                        var diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                        var startOfWeek = today.AddDays(-diff);
                        var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                        totalDistance = activities
                            .Where(a => a.ActivityId == medal.ActivityId && a.DateStart >= startOfWeek && a.DateStart <= endOfWeek)
                            .Sum(a => a.Distance);
                        break;
                    }
                case "Once":
                    // Once progress calculation
                    totalDistance = activities
                        .Where(a => a.ActivityId == medal.ActivityId)
                        .Sum(a => a.Distance);
                    break;
                case "Monthly":
                    // Placeholder for monthly logic, can be customized later
                    totalDistance = 0;
                    break;
                default:
                    return 0;
            }

            // Calculate progress as percentage, for "Once" medals, you accumulate the total distance.
            double progress = (totalDistance / medal.TargetValue) * 100;
            return Math.Min(100, (int)Math.Round(progress));
        }
    }
}
