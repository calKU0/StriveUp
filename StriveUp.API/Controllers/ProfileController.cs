using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs.Profile;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/user/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(string userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserActivities)
                    .Include(u => u.MedalsEarned).ThenInclude(me => me.Medal)
                    .Where(u => u.Id == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                var userProfile = _mapper.Map<UserProfileDto>(user);

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Message = "An internal server error occurred while fetching the profile.", Error = ex.Message });
            }
        }


        [HttpPut]
        public async Task<ActionResult> UpdateProfile([FromBody] EditUserProfileDto profile)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User not authenticated." });
                }

                var user = await _context.Users.FindAsync(userId);

                if (string.IsNullOrEmpty(profile.UserName) || string.IsNullOrEmpty(profile.FirstName) || string.IsNullOrEmpty(profile.LastName))
                {
                    return BadRequest(new { Message = "All fields are required." });
                }

                _mapper.Map(profile, user);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Message = "An internal server error occurred while updating the profile.", Error = ex.Message });
            }
        }
    }

}
