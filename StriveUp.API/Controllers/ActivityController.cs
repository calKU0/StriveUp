using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using System.Security.Claims;
using StriveUp.Infrastructure.Identity;
using StriveUp.API.Interfaces;

namespace StriveUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ActivityController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILevelService _levelService;
        private readonly INotificationService _notificationService;

        public ActivityController(AppDbContext context, IMapper mapper, ILevelService levelService, UserManager<AppUser> userManager, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _levelService = levelService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpPost("addActivity")]
        public async Task<IActionResult> AddActivity(CreateUserActivityDto dto)
        {
            var callingUserId = GetUserId();
            var callingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == callingUserId);

            if (callingUserId == null) return Unauthorized();

            try
            {
                var isAdmin = await _userManager.IsInRoleAsync(callingUser, "Admin");

                if (!string.IsNullOrEmpty(dto.UserId))
                {
                    if (!isAdmin)
                    {
                        return Forbid();
                    }
                }
                else
                {
                    dto.UserId = callingUserId;
                }

                var userActivity = _mapper.Map<UserActivity>(dto);

                if (!string.IsNullOrEmpty(userActivity.SynchroId))
                {
                    var duplicate = await _context.UserActivities
                        .AnyAsync(ua => ua.SynchroId == userActivity.SynchroId);

                    if (duplicate)
                    {
                        return Conflict("This activity has already been synchronized.");
                    }
                }

                var user = await _context.Users
                    .Include(u => u.Level)
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                var activity = await _context.Activities.FindAsync(dto.ActivityId);
                if (activity == null)
                    return BadRequest("Invalid activity ID.");

                double durationSeconds = (dto.DateEnd - dto.DateStart).TotalSeconds;

                userActivity.DurationSeconds = durationSeconds;
                userActivity.CaloriesBurned = Convert.ToInt32(Math.Round((activity.AverageCaloriesPerHour / 3600.0) * durationSeconds));

                userActivity.MaxSpeed = userActivity.SpeedData.Count > 0 && userActivity.MaxSpeed == null
                    ? userActivity.SpeedData.Max(s => s.SpeedValue)
                    : userActivity.MaxSpeed;

                userActivity.AvarageSpeed = userActivity.SpeedData.Count > 0 && userActivity.AvarageSpeed == null
                    ? userActivity.SpeedData.Average(s => s.SpeedValue)
                    : userActivity.AvarageSpeed;

                userActivity.AvarageHr = userActivity.HrData.Count > 0 && userActivity.AvarageHr == null
                    ? Convert.ToInt32(userActivity.HrData.Average(s => s.HearthRateValue))
                    : userActivity.AvarageHr;

                userActivity.MaxHr = userActivity.HrData.Count > 0 && userActivity.MaxHr == null
                    ? userActivity.HrData.Max(s => s.HearthRateValue)
                    : userActivity.MaxHr;

                var activityConfig = await _context.ActivityConfig
                    .FirstOrDefaultAsync(ac => ac.ActivityId == dto.ActivityId);

                if (activityConfig != null)
                {
                    // Add XP 
                    int xpReward = activityConfig.PointsPerMinute > 0
                        ? Convert.ToInt32(Math.Round(activityConfig.PointsPerMinute * userActivity.DurationSeconds / 60))
                        : 1 * Convert.ToInt32(Math.Round(userActivity.DurationSeconds / 60));
                    user.CurrentXP += xpReward;
                }

                _context.UserActivities.Add(userActivity);

                // If activity is synchronized, notify the user
                if (userActivity.isSynchronized)
                {
                    var notifDto = new CreateNotificationDto
                    {
                        UserId = user.Id,
                        ActorId = user.Id, // the one who added it
                        Title = "New Activity Synchronized",
                        Message = $"The activity {userActivity.Title} has been synced to your account. Click to review your performance!",
                        Type = "sync",
                        RedirectUrl = $"/activity/{userActivity.Id}"
                    };

                    await _notificationService.CreateNotificationAsync(notifDto);
                }

                await _levelService.UpdateUserLevelAsync(user);
                await _context.SaveChangesAsync();

                return Ok(userActivity.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("userActivities")]
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetActivities([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
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
                    .Include(ua => ua.ActivityComments)!.ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .OrderByDescending(ua => ua.DateStart)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var activityDtos = _mapper.Map<List<UserActivityDto>>(activities);

                foreach (var dto in activityDtos)
                {
                    var entity = activities.First(a => a.Id == dto.Id);
                    if (entity.ActivityLikes != null)
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
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
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
                    .Include(ua => ua.ActivityComments)!.ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .OrderByDescending(ua => ua.DateStart)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var activityDtos = _mapper.Map<List<UserActivityDto>>(activities);

                foreach (var dto in activityDtos)
                {
                    var entity = activities.First(a => a.Id == dto.Id);

                    if (entity.ActivityLikes != null)
                        dto.IsLikedByCurrentUser = entity.ActivityLikes.Any(l => l.UserId == userId);
                    if (entity.Route != null)
                        dto.Route = _mapper.Map<List<GeoPointDto>>(entity.Route.OrderBy(p => p.Timestamp));

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


        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserActivityDto>> GetActivityById(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var activity = await _context.UserActivities
                    .AsSplitQuery()
                    .Where(ua => ua.Id == id)
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments)!.ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .FirstOrDefaultAsync();

                if (activity == null)
                    return NotFound("Activity not found or access denied.");

                var dto = _mapper.Map<UserActivityDto>(activity);
                if (activity.ActivityLikes != null)
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

        [HttpGet("activityConfig/{id}")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivityConfig(int id)
        {
            try
            {
                var activity = await _context.Activities
                    .Include(a => a.Config)
                    .Where(a => a.Id == id)
                    .FirstOrDefaultAsync();

                var activityDto = _mapper.Map<ActivityDto>(activity);
                return Ok(activityDto);
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
                // Get the current user (the actor)
                var actorUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (actorUser == null) return Unauthorized();

                // Get the owner of the activity (destination user)
                var destinationUserId = await _context.UserActivities
                    .Where(a => a.Id == activityId)
                    .Select(a => a.UserId)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(destinationUserId))
                    return BadRequest("Invalid activity ID.");

                // Check if the like already exists
                var existingLike = await _context.ActivityLikes
                    .FirstOrDefaultAsync(l => l.UserActivityId == activityId && l.UserId == userId);

                if (existingLike == null)
                {
                    // Add the like
                    _context.ActivityLikes.Add(new ActivityLike
                    {
                        UserActivityId = activityId,
                        UserId = userId,
                        LikedAt = DateTime.UtcNow
                    });

                    // Only notify if user is not liking their own activity
                    if (userId != destinationUserId)
                    {
                        var notifDto = new CreateNotificationDto
                        {
                            UserId = destinationUserId,      // The owner of the activity
                            ActorId = userId,                // The liker
                            Title = "New Like",
                            Message = $"{actorUser.UserName} liked your activity.",
                            Type = "like",
                            RedirectUrl = $"/activity/{activityId}"
                        };

                        await _notificationService.CreateNotificationAsync(notifDto);
                    }
                }
                else
                {
                    // Remove the like (unlike)
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
                // Add the comment
                var comment = new ActivityComment
                {
                    UserActivityId = activityId,
                    UserId = userId,
                    Content = dto.Content
                };

                _context.ActivityComments.Add(comment);
                await _context.SaveChangesAsync();

                // Get actor user (commenter)
                var actorUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (actorUser == null) return Unauthorized();

                // Get owner of the activity
                var destinationUserId = await _context.UserActivities
                    .Where(a => a.Id == activityId)
                    .Select(a => a.UserId)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(destinationUserId))
                    return BadRequest("Invalid activity ID.");

                // Avoid notifying yourself
                if (userId != destinationUserId)
                {
                    var notifDto = new CreateNotificationDto
                    {
                        UserId = destinationUserId,
                        ActorId = userId,
                        Title = "New Comment",
                        Message = $"{actorUser.UserName} commented on your activity.",
                        Type = "comment",
                        RedirectUrl = $"/activity/{activityId}/comments"
                    };

                    await _notificationService.CreateNotificationAsync(notifDto);
                }

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
