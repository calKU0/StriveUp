using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Shared.DTOs;
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

        [HttpGet("{userName}")]
        public async Task<ActionResult<UserProfileDto>> GetProfile(string userName)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var user = await _context.Users
                    .AsNoTracking()  // AsNoTracking to improve performance for read-only queries
                    .Where(u => u.UserName == userName)
                    .Include(u => u.Level)
                    .Include(u => u.MedalsEarned)
                        .ThenInclude(me => me.Medal)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                var userActivities = await _context.UserActivities
                    .AsSplitQuery() // Use AsSplitQuery to break the query into multiple
                    .Where(ua => ua.UserId == user.Id)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments)
                        .ThenInclude(c => c.User)
                    .Include(ua => ua.Activity)
                    .ToListAsync();

                user.UserActivities = userActivities;           

                var userProfile = _mapper.Map<UserProfileDto>(user);

                userProfile.Activities = userProfile.Activities
                    ?.OrderByDescending(a => a.DateStart)
                    .ToList();

                // Optimize lookup by creating a dictionary from user.UserActivities
                var activityLookup = user.UserActivities
                    .ToDictionary(a => a.Id);

                foreach (var activityDto in userProfile.Activities ?? Enumerable.Empty<UserActivityDto>())
                {
                    if (activityLookup.TryGetValue(activityDto.Id, out var activityEntity))
                    {
                        activityDto.IsLikedByCurrentUser = activityEntity.ActivityLikes
                            .Any(l => l.UserId == currentUserId);

                        activityDto.Comments = _mapper.Map<List<ActivityCommentDto>>(activityEntity.ActivityComments);
                    }
                }

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
