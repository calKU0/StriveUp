
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ActivityController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ActivityController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("addActivity")]
        public async Task<IActionResult> AddActivity(CreateUserActivityDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activity = await _context.Activities.FindAsync(dto.ActivityId);
                if (activity == null)
                    return BadRequest("Invalid activity ID.");

                var userActivity = _mapper.Map<UserActivity>(dto);
                userActivity.UserId = userId!;
                userActivity.CaloriesBurned = Convert.ToInt32(Math.Round((double)(activity.AverageCaloriesPerHour / 3600) * dto.DurationSeconds));

                _context.UserActivities.Add(userActivity);
                await _context.SaveChangesAsync();

                return Ok(userActivity.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("activities")]
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetActivities()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activities = await _context.UserActivities
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments).ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .Where(ua => ua.UserId == userId)
                    .ToListAsync();

                var activityDtos = _mapper.Map<List<UserActivityDto>>(activities);

                foreach (var dto in activityDtos)
                {
                    var entity = activities.First(a => a.Id == dto.Id);
                    dto.IsLikedByCurrentUser = entity.ActivityLikes.Any(l => l.UserId == userId);
                    dto.Comments = _mapper.Map<List<ActivityCommentDto>>(entity.ActivityComments);
                }

                return Ok(activityDtos);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("feed")]
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetFeed()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                    return Unauthorized();

                // Get IDs of users you follow
                var followedUserIds = await _context.UserFollowers
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FollowedId)
                    .ToListAsync();

                // Include own ID
                var userIds = followedUserIds
                    .Append(userId)
                    .Distinct()
                    .ToList();

                // Fetch activities from followed users and yourself
                var activities = await _context.UserActivities
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.User)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments)
                        .ThenInclude(c => c.User)
                    .Where(ua => userIds.Contains(ua.UserId))
                    .OrderByDescending(ua => ua.DateStart)
                    .ToListAsync();

                var activityDtos = _mapper.Map<List<UserActivityDto>>(activities);

                foreach (var dto in activityDtos)
                {
                    var entity = activities.First(a => a.Id == dto.Id);
                    dto.IsLikedByCurrentUser = entity.ActivityLikes.Any(l => l.UserId == userId);
                    dto.Comments = _mapper.Map<List<ActivityCommentDto>>(entity.ActivityComments);
                    dto.Route = _mapper.Map<List<GeoPointDto>>(entity.Route.OrderBy(p => p.Timestamp));
                }

                return Ok(activityDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("activity/{id:int}")]
        public async Task<ActionResult<UserActivityDto>> GetActivityById(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activity = await _context.UserActivities
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments).ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .FirstOrDefaultAsync(ua => ua.Id == id && ua.UserId == userId);

                if (activity == null)
                    return NotFound("Activity not found or access denied.");

                var dto = _mapper.Map<UserActivityDto>(activity);
                dto.IsLikedByCurrentUser = activity.ActivityLikes.Any(l => l.UserId == userId);
                dto.Comments = _mapper.Map<List<ActivityCommentDto>>(activity.ActivityComments);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("availableActivities")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAvaliableActivities()
        {
            try
            {
                var activities = await _context.Activities
                    .Include(a => a.Config)
                    .ToListAsync();
                var activityDtos = _mapper.Map<List<ActivityDto>>(activities);
                return Ok(activityDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("like/{activityId}")]
        public async Task<IActionResult> LikeActivity(int activityId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var existingLike = await _context.ActivityLikes
                    .FirstOrDefaultAsync(l => l.UserActivityId == activityId && l.UserId == userId);

                if (existingLike == null)
                {
                    _context.ActivityLikes.Add(new ActivityLike
                    {
                        UserActivityId = activityId,
                        UserId = userId,
                        LikedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    _context.ActivityLikes.Remove(existingLike);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("comment/{activityId}")]
        public async Task<IActionResult> AddComment(int activityId, AddCommentDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var comment = new ActivityComment
                {
                    UserActivityId = activityId,
                    UserId = userId,
                    Content = dto.Content
                };

                _context.ActivityComments.Add(comment);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("activityComments/{activityId}")]
        public async Task<ActionResult<IEnumerable<ActivityCommentDto>>> GetActivityComments(int activityId)
        {
            try
            {
                var comments = await _context.ActivityComments
                    .Include(c => c.User)
                    .Where(c => c.UserActivityId == activityId)
                    .ToListAsync();

                var commentsDtos = _mapper.Map<List<ActivityCommentDto>>(comments);
                return Ok(commentsDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
