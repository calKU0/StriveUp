using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs;

namespace StriveUp.API.Controllers
{
    [ApiController]
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
                return StatusCode(500, "Internal server error");
            }
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
