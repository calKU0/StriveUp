using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MedalController : Controller
    {
        private readonly AppDbContext _context;

        public MedalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("medals")]
        public async Task<ActionResult<IEnumerable<MedalDto>>> GetAllMedals()
        {
            try
            {
                return await _context.Medals
                    .Select(m => new MedalDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        ImageUrl = m.ImageUrl,
                        Level = m.Level,
                        TargetValue = m.TargetValue,
                        Frequency = m.Frequency,
                    })
                    .ToListAsync();
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
                return await _context.MedalsEarned
                    .Where(m => m.UserId == userId)
                    .Include(m => m.Medal)
                    .Select(m => new MedalDto
                    {
                        Id = m.Id,
                        Name = m.Medal.Name,
                        Description = m.Medal.Description,
                        ImageUrl = m.Medal.ImageUrl,
                        Level = m.Medal.Level,
                        TargetValue = m.Medal.TargetValue,
                        Frequency = m.Medal.Frequency,
                        DateEarned = m.DateEarned
                    })
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("userNotEarnedMedals")]
        public async Task<IEnumerable<MedalDto>> GetNotEarnedMedalsAsync(string userId)
        {
            var userMedals = await _context.MedalsEarned
                .Where(me => me.UserId == userId)
                .Select(me => me.MedalId)
                .ToListAsync();

            return await _context.Medals
                .Where(m => !userMedals.Contains(m.Id))
                .Select(m => new MedalDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl,
                    Level = m.Level,
                    TargetValue = m.TargetValue,
                    Frequency = m.Frequency,
                })
                .ToListAsync();
        }
    }
}
