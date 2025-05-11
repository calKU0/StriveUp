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

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost("addActivity")]
        public async Task<IActionResult> AddActivity(CreateUserActivityDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var activity = await _context.Activities.FindAsync(dto.ActivityId);
                if (activity == null)
                    return BadRequest("Invalid activity ID.");

                var userActivity = _mapper.Map<UserActivity>(dto);
                userActivity.UserId = userId;
                userActivity.CaloriesBurned = Convert.ToInt32(Math.Round((double)(activity.AverageCaloriesPerHour / 3600) * dto.DurationSeconds));
                userActivity.MaxSpeed = userActivity.SpeedData?.Any() == true 
                    ? userActivity.SpeedData.Max(s => s.SpeedValue) 
                    : null;
                userActivity.AvarageSpeed = userActivity.SpeedData?.Any() == true
                    ? userActivity.SpeedData.Average(s => s.SpeedValue)
                    : null;
                userActivity.AvarageHr = userActivity.HrData?.Any() == true
                    ? Convert.ToInt32(userActivity.HrData.Average(s => s.HearthRateValue))
                    : null;
                userActivity.MaxHr = userActivity.HrData?.Any() == true
                    ? userActivity.HrData.Max(s => s.HearthRateValue)
                    : null;


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
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var activities = await _context.UserActivities
                    .AsSplitQuery()
                    .Where(ua => ua.UserId == userId)
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments).ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
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
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var followedUserIds = await _context.UserFollowers
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FollowedId)
                    .ToListAsync();

                var userIds = followedUserIds.Append(userId).Distinct().ToList();

                var activities = await _context.UserActivities
                    .AsSplitQuery()
                    .Where(ua => userIds.Contains(ua.UserId))
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments).ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserActivityDto>> GetActivityById(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var activity = await _context.UserActivities
                    .AsSplitQuery()
                    .Where(ua => ua.Id == id && ua.UserId == userId)
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments).ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .FirstOrDefaultAsync();

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
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAvailableActivities()
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
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
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
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
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

                var commentDtos = _mapper.Map<List<ActivityCommentDto>>(comments);
                return Ok(commentDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
