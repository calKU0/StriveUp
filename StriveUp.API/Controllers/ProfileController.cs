using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(string userId)
        {
            var headers = Request.Headers;

            Console.WriteLine("Request Headers: ");
            foreach (var header in headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            var activities = await _context.UserActivities
                .Include(ua => ua.Activity)
                .Where(ua => ua.UserId == userId)
                .Select(ua => new UserActivityDto
                {
                    ActivityId = ua.Activity.Id,
                    Title = ua.Activity.Name,
                    Description = ua.Activity.Description,
                    DurationMinutes = ua.DurationMinutes
                })
                .ToListAsync();

            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Activities = activities
            };

            return Ok(userProfile);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateProfile(string userId, [FromBody] UserProfileDto profile)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = profile.FirstName;
            user.Email = profile.Email;
            user.UserName = profile.UserName;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
