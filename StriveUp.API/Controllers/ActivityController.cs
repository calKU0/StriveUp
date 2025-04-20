
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ActivityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("addUserActivity")]
        public async Task<IActionResult> AddUserActivity(UserActivityDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activity = await _context.Activities.FindAsync(dto.ActivityId);
            if (activity == null)
            {
                return BadRequest("Invalid activity ID.");
            }

            var calories = Convert.ToInt32(Math.Round((decimal)(activity.AverageCaloriesPerHour / 60) * dto.DurationMinutes));

            var userActivity = new UserActivity
            {
                UserId = userId!,
                Title = dto.Title,
                ActivityId = dto.ActivityId,
                Description = dto.Description,
                //ImageUrls = dto.ImageUrls ?? new List<string>(),
                DurationMinutes = dto.DurationMinutes,
                DateStart = dto.DateStart,
                DateEnd = dto.DateEnd,
                CaloriesBurned = calories
            };

            _context.UserActivities.Add(userActivity);
            await _context.SaveChangesAsync();

            return Ok(userActivity);
        }

        [HttpGet("userActivities")]
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetUserActivities()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var activities = await _context.UserActivities
                    .Include(ua => ua.Activity)
                    //.Include(ua => ua.ActivityLikes)
                    //.Include(ua => ua.ActivityComments)
                    .Where(ua => ua.UserId == userId)
                    .ToListAsync();

                var activityDtos = activities
                    .Select(ua => new UserActivityDto
                    {
                        Id = ua.Id,
                        ActivityId = ua.ActivityId,
                        ActivityName = ua.Activity.Name,
                        Title = ua.Activity.Name,
                        Description = ua.Description,
                        DurationMinutes = ua.DurationMinutes,
                        DateStart = ua.DateStart,
                        DateEnd = ua.DateEnd,
                        CaloriesBurned = ua.CaloriesBurned,
                        UserName = ua.User.UserName,
                        LikeCount = ua.ActivityLikes.Count,
                        IsLikedByCurrentUser = ua.ActivityLikes.Any(like => like.UserId == userId),
                        Comments = ua.ActivityComments
                            .OrderBy(c => c.CreatedAt)
                            .Select(c => new CommentDto
                            {
                                UserName = c.User.UserName,
                                Content = c.Content,
                                CreatedAt = c.CreatedAt
                            }).ToList()
                        // ImageUrls = ua.ImageUrls
                    }).ToList();

                return Ok(activityDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("userFeed")]
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetUserFeed()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var friends = await _context.Friendships
                    .Where(f => f.UserId == userId)
                    .Select(f => f.FriendId)
                    .ToListAsync();

                var userIds = friends.Append(userId).ToList(); // własne + znajomi

                var activities = await _context.UserActivities
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments)
                        .ThenInclude(c => c.User)
                    .Where(ua => userIds.Contains(ua.UserId))
                    .ToListAsync();

                var activityDtos = activities.Select(ua => new UserActivityDto
                {
                    Id = ua.Id,
                    ActivityId = ua.ActivityId,
                    ActivityName = ua.Activity.Name,
                    Title = ua.Title,
                    Description = ua.Description,
                    DurationMinutes = ua.DurationMinutes,
                    DateStart = ua.DateStart,
                    DateEnd = ua.DateEnd,
                    CaloriesBurned = ua.CaloriesBurned,
                    UserName = ua.User.UserName,
                    LikeCount = ua.ActivityLikes.Count,
                    IsLikedByCurrentUser = ua.ActivityLikes.Any(like => like.UserId == userId),
                    Comments = ua.ActivityComments
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => new CommentDto
                        {
                            UserName = c.User.UserName,
                            Content = c.Content,
                            CreatedAt = c.CreatedAt
                        }).ToList()
                }).ToList();

                return Ok(activityDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("availableActivities")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities()
        {
            try
            {
                var activities = await _context.Activities
                    .Select(a => new ActivityDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToListAsync();

                return Ok(activities);
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
    }
}
