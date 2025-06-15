using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.API.Interfaces;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using StriveUp.Shared.DTOs.Leaderboard;
using System.Security.Claims;

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

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateUserActivityDto dto)
        {
            var activity = await _context.UserActivities.FindAsync(id);
            if (activity == null)
                return NotFound();

            // Map updated fields
            activity.Title = dto.Title;
            activity.Description = dto.Description;
            activity.ActivityId = dto.ActivityId;
            activity.IsPrivate = dto.IsPrivate;

            activity.ShowSpeed = dto.ShowSpeed;
            activity.ShowHeartRate = dto.ShowHeartRate;
            activity.ShowCalories = dto.ShowCalories;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.UserActivities
                       .Include(a => a.BestEfforts)
                       .Include(a => a.ActivityComments)
                       .Include(a => a.ActivityLikes)
                       .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound();

            // Remove related entities first
            _context.BestEfforts.RemoveRange(activity.BestEfforts);
            _context.ActivityComments.RemoveRange(activity.ActivityComments);
            _context.ActivityLikes.RemoveRange(activity.ActivityLikes);

            // Remove the activity itself
            _context.UserActivities.Remove(activity);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("addActivity")]
        public async Task<IActionResult> AddActivity(CreateUserActivityDto dto)
        {
            var callingUserId = GetUserId();
            var callingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == callingUserId);

            if (callingUserId == null) return Unauthorized();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var isAdmin = await _userManager.IsInRoleAsync(callingUser, "Admin");

                if (!string.IsNullOrEmpty(dto.UserId))
                {
                    if (!isAdmin)
                        return Forbid();
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
                        return Conflict("This activity has already been synchronized.");
                }

                var user = await _context.Users
                    .Include(u => u.Level)
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                var activity = await _context.Activities.FindAsync(dto.ActivityId);
                if (activity == null)
                    return BadRequest("Invalid activity ID.");

                double durationSeconds = (dto.DateEnd - dto.DateStart).TotalSeconds;

                if (userActivity.CaloriesBurned == null || userActivity.CaloriesBurned == 0)
                {
                    userActivity.CaloriesBurned = Convert.ToInt32(Math.Round((activity.AverageCaloriesPerHour / 3600.0) * durationSeconds));
                }

                userActivity.DurationSeconds = durationSeconds;

                userActivity.MaxSpeed ??= userActivity.SpeedData.Count > 0
                    ? userActivity.SpeedData.Max(s => s.SpeedValue)
                    : null;

                userActivity.AvarageSpeed ??= userActivity.SpeedData.Count > 0
                    ? userActivity.SpeedData.Average(s => s.SpeedValue)
                    : null;

                userActivity.AvarageHr ??= userActivity.HrData.Count > 0
                    ? Convert.ToInt32(userActivity.HrData.Average(s => s.HearthRateValue))
                    : null;

                userActivity.MaxHr ??= userActivity.HrData.Count > 0
                    ? userActivity.HrData.Max(s => s.HearthRateValue)
                    : null;

                if (userActivity.ElevationData != null && userActivity.ElevationData.Count > 0 && userActivity.ElevationGain == null)
                {
                    double gain = 0;
                    for (int i = 1; i < userActivity.ElevationData.Count; i++)
                    {
                        double delta = userActivity.ElevationData[i].ElevationValue - userActivity.ElevationData[i - 1].ElevationValue;
                        if (delta > 0)
                            gain += delta;
                    }
                    userActivity.ElevationGain = Convert.ToInt32(gain);
                }

                var activityConfig = await _context.ActivityConfig.FirstOrDefaultAsync(ac => ac.ActivityId == dto.ActivityId);
                var userConfig = await _context.UserConfigs.FirstOrDefaultAsync(ac => ac.UserId == user!.Id);

                userActivity.IsPrivate = userConfig!.PrivateActivities;

                if (activityConfig != null)
                {
                    int xpReward = activityConfig.PointsPerMinute > 0
                        ? Convert.ToInt32(Math.Round(activityConfig.PointsPerMinute * userActivity.DurationSeconds / 60))
                        : Convert.ToInt32(Math.Round(userActivity.DurationSeconds / 60));
                    user.CurrentXP += xpReward;
                }

                List<SegmentConfig> segments = new();
                double[] cumulativeDistances = Array.Empty<double>();

                if (userActivity.Route is not null && userActivity.Route.Count > 1)
                {
                    segments = await _context.SegmentConfigs.Where(s => s.ActivityId == userActivity.ActivityId).ToListAsync();
                    cumulativeDistances = ComputeCumulativeDistances(userActivity.Route);
                    var splits = ComputeActivitySplits(
                        userActivity.Route,
                        userActivity.HrData,
                        userActivity.SpeedData,
                        userActivity.ElevationData,
                        cumulativeDistances,
                        activityConfig.MeasurementType
                    );

                    userActivity.Splits = splits;
                }
                _context.UserActivities.Add(userActivity);

                await _context.SaveChangesAsync();

                foreach (var segment in segments)
                {
                    var bestSegment = GetBestSegmentFromGpsRoute(
                        segment.Id,
                        userActivity.UserId,
                        userActivity.Id,
                        userActivity.DateStart,
                        userActivity.Route,
                        segment.DistanceMeters,
                        userActivity.Distance,
                        cumulativeDistances);

                    if (bestSegment != null)
                    {
                        _context.BestEfforts.Add(bestSegment);
                    }
                }

                if (userActivity.isSynchronized)
                {
                    userActivity.IsPrivate = true; // Automatically set to private if synchronized - Strava TOS

                    var notifDto = new CreateNotificationDto
                    {
                        UserId = user.Id,
                        ActorId = user.Id,
                        Title = "New Activity Synchronized",
                        Message = $"The activity {userActivity.Title} has been synced to your account. Click to review your performance!",
                        Type = "sync",
                        RedirectUrl = $"/activity/{userActivity.Id}"
                    };

                    await _notificationService.CreateNotificationAsync(notifDto);
                }

                await _levelService.UpdateUserLevelAsync(user);

                // Final save and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(userActivity.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("userActivities")]
        public async Task<ActionResult<IEnumerable<SimpleUserActivityDto>>> GetActivities([FromQuery] string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                // Fetch target user and their config first
                var targetUser = await _context.Users
                    .Include(u => u.UserConfig)
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (targetUser == null) return NotFound("User not found");

                // Check if calling user is NOT the target user
                bool isCallerSameUser = targetUser.Id == userId;

                if (!isCallerSameUser)
                {
                    // If activities are private, deny fetching activities
                    if (targetUser.UserConfig.PrivateActivities == true)
                    {
                        return StatusCode(403, "User's activities are private.");
                    }
                }

                // Query activities for target user
                var query = _context.UserActivities
                    .AsSplitQuery()
                    .Where(ua => ua.User.UserName == userName)
                    .Where(ua => isCallerSameUser || !ua.IsPrivate)
                    .OrderByDescending(ua => ua.DateStart)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ua => new SimpleUserActivityDto
                    {
                        Id = ua.Id,
                        ActivityId = ua.Activity.Id,
                        UserAvatar = ua.User.Avatar,
                        Title = ua.Title,
                        Description = ua.Description,
                        DurationSeconds = ua.DurationSeconds,
                        Distance = ua.Distance,
                        CaloriesBurned = ua.CaloriesBurned,
                        AvarageSpeed = ua.AvarageSpeed,
                        UserName = ua.User.UserName!,
                        DateStart = ua.DateStart,
                        DateEnd = ua.DateEnd,
                        ActivityName = ua.Activity.Name,
                        SynchroProviderName = ua.SynchroProviderName,
                        // If PrivateMap is true and caller is NOT the user, exclude route
                        Route = (!isCallerSameUser && targetUser.UserConfig.PrivateMap == true) ? null : ua.Route.Select(p => new GeoPointDto
                        {
                            Latitude = p.Latitude,
                            Longitude = p.Longitude,
                            Timestamp = p.Timestamp
                        }).ToList(),
                        Comments = ua.ActivityComments.Select(c => new ActivityCommentDto
                        {
                            Content = c.Content,
                            CreatedAt = c.CreatedAt,
                            UserName = c.User.UserName
                        }).ToList(),
                        IsLikedByCurrentUser = ua.ActivityLikes.Any(l => l.UserId == userId),
                        LikeCount = ua.ActivityLikes.Count
                    });

                var activities = await query.ToListAsync();

                return Ok(activities);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("feed")]
        public async Task<ActionResult<IEnumerable<SimpleUserActivityDto>>> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var followedUserIds = await _context.UserFollowers
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FollowedId)
                    .ToListAsync();

                var userIds = followedUserIds.Append(userId);

                // Fetch activities with necessary includes for UserConfig and relations
                var activities = await _context.UserActivities
                    .AsSplitQuery()
                    .Where(ua => userIds.Contains(ua.UserId))
                    .Include(ua => ua.User).ThenInclude(u => u.UserConfig)
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.ActivityComments).ThenInclude(c => c.User)
                    .Include(ua => ua.ActivityLikes)
                    .OrderByDescending(ua => ua.DateStart)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Filter out activities from users with PrivateActivities true (unless it's the caller's own)
                var filteredActivities = activities
                    .Where(ua =>
                        ua.User.Id == userId || // the activity belongs to the current user
                        (ua.User.UserConfig.PrivateActivities != true && ua.IsPrivate == false) // from a public user and activity is public
                    )
                    .ToList();

                // Map to DTO in memory with PrivateMap route exclusion
                var result = filteredActivities.Select(ua => new SimpleUserActivityDto
                {
                    Id = ua.Id,
                    ActivityId = ua.Activity.Id,
                    UserAvatar = ua.User.Avatar,
                    UserId = ua.User.Id,
                    UserName = ua.User.UserName!,
                    Title = ua.Title,
                    Description = ua.Description,
                    DurationSeconds = ua.DurationSeconds,
                    Distance = ua.Distance,
                    CaloriesBurned = ua.CaloriesBurned,
                    AvarageSpeed = ua.AvarageSpeed,
                    DateStart = ua.DateStart,
                    DateEnd = ua.DateEnd,
                    ActivityName = ua.Activity.Name,
                    SynchroProviderName = ua.SynchroProviderName,
                    HasNewRecord = CheckIfActivityHasNewRecord(ua.Id),
                    Route = (ua.User.Id == userId || ua.User.UserConfig.PrivateMap != true)
                        ? ua.Route.Select(p => new GeoPointDto
                        {
                            Latitude = p.Latitude,
                            Longitude = p.Longitude,
                            Timestamp = p.Timestamp
                        }).ToList()
                        : null,
                    Comments = ua.ActivityComments.Select(c => new ActivityCommentDto
                    {
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UserName = c.User.UserName
                    }).ToList(),
                    IsLikedByCurrentUser = ua.ActivityLikes.Any(l => l.UserId == userId),
                    LikeCount = ua.ActivityLikes.Count
                }).ToList();

                return Ok(result);
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
                    .Include(ua => ua.Activity)
                    .Include(ua => ua.User).ThenInclude(u => u.UserConfig)
                    .Include(ua => ua.ActivityLikes)
                    .Include(ua => ua.ActivityComments)!.ThenInclude(c => c.User)
                    .Include(ua => ua.Route)
                    .Include(ua => ua.HrData)
                    .Include(ua => ua.SpeedData)
                    .Include(ua => ua.ElevationData)
                    .Include(ua => ua.Splits)
                    .Include(ua => ua.BestEfforts).ThenInclude(be => be.SegmentConfig)
                    .FirstOrDefaultAsync(ua => ua.Id == id);

                if (activity == null)
                    return NotFound("Activity not found or access denied.");

                // Check if calling user is NOT the owner of the activity
                bool isCallerOwner = activity.User.Id == userId;
                if (!isCallerOwner && activity.IsPrivate)
                {
                    return Forbid("This activity is private.");
                }

                var dto = _mapper.Map<UserActivityDto>(activity);

                // Load all best efforts for this user that belong to the same activity type (segmentConfig.ActivityId == activity.Activity.Id)
                var allBestEffortsForActivityType = await _context.BestEfforts
                    .AsNoTracking()
                    .Include(be => be.SegmentConfig)
                    .Where(be => be.UserId == userId && !be.UserActivity.IsPrivate)
                    .Where(be => be.SegmentConfig.ActivityId == activity.Activity.Id)
                    .OrderBy(be => be.DurationSeconds)
                    .ToListAsync();

                // Group all efforts by segment short name
                var effortsGroupedBySegment = allBestEffortsForActivityType
                    .GroupBy(be => be.SegmentConfig.ShortName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Take(3).ToList() // top 3 per segment by duration
                    );

                var bestEffortsFromThisActivityTop3 = new List<UserBestEffortsStatsDto>();

                // For each effort in the current activity, check if it is in the top 3 efforts for that segment
                foreach (var effort in activity.BestEfforts)
                {
                    var segmentShortName = effort.SegmentConfig.ShortName;

                    if (effortsGroupedBySegment.TryGetValue(segmentShortName, out var top3Efforts))
                    {
                        // Check if this activity's effort is among the top 3 by matching UserActivityId and DurationSeconds (with small tolerance)
                        var rank = top3Efforts.FindIndex(be =>
                            be.UserActivityId == effort.UserActivityId &&
                            Math.Abs(be.DurationSeconds - effort.DurationSeconds) < 0.001) + 1;

                        if (rank > 0 && rank <= 3)
                        {
                            bestEffortsFromThisActivityTop3.Add(new UserBestEffortsStatsDto
                            {
                                SegmentName = effort.SegmentConfig.Name,
                                SegmentShortName = segmentShortName,
                                TotalDuration = effort.DurationSeconds,
                                Speed = effort.Speed,
                                ActivityDate = effort.ActivityDate,
                                ActivityId = effort.UserActivityId,
                                SegmentRank = rank
                            });
                        }
                    }
                }

                // Assign only top 3 efforts from this activity to DTO
                dto.BestEfforts = bestEffortsFromThisActivityTop3;

                // If PrivateMap is true and caller is not owner, exclude Route data
                if (!isCallerOwner && activity.User.UserConfig.PrivateMap == true)
                {
                    dto.Route = null;
                }

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

        [HttpGet("activities-with-segments")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivitiesWithSegments()
        {
            try
            {
                var activities = await _context.Activities
                    .Include(a => a.Config)
                    .Include(a => a.SegmentConfigs)
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
                            Message = "liked your activity.",
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
                        Message = "commented on your activity.",
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

        private BestEffort? GetBestSegmentFromGpsRoute(
            int segmentId,
            string userId,
            int activityId,
            DateTime activityDate,
            List<GeoPoint> route,
            double targetDistanceMeters,
            double activityDistanceMeters,
            double[] cumulativeDistances)
        {
            if (route == null || route.Count < 2)
                return null;

            double totalRouteDistanceMeters = cumulativeDistances[^1];

            if (activityDistanceMeters < targetDistanceMeters)
                return null;

            if (totalRouteDistanceMeters < targetDistanceMeters)
                return null;

            double bestDuration = double.MaxValue;
            BestEffort? bestSegment = null;
            try
            {
                for (int startIndex = 0; startIndex < route.Count - 1; startIndex++)
                {
                    for (int endIndex = startIndex + 1; endIndex < route.Count; endIndex++)
                    {
                        if (endIndex < cumulativeDistances.Length && startIndex < cumulativeDistances.Length)
                        {
                            double segmentDistance = cumulativeDistances[endIndex] - cumulativeDistances[startIndex];

                            if (segmentDistance >= targetDistanceMeters)
                            {
                                var duration = (route[endIndex].Timestamp - route[startIndex].Timestamp).TotalSeconds;

                                if (duration > 0 && duration < bestDuration)
                                {
                                    bestDuration = duration;
                                    bestSegment = new BestEffort
                                    {
                                        SegmentConfigId = segmentId,
                                        UserId = userId,
                                        UserActivityId = activityId,
                                        ActivityDate = activityDate,
                                        DurationSeconds = duration,
                                    };
                                }
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("ASDASDASd");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return bestSegment;
        }

        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180);

        private double[] ComputeCumulativeDistances(List<GeoPoint> route)
        {
            var cumulativeDistances = new double[route.Count];
            cumulativeDistances[0] = 0;
            for (int i = 1; i < route.Count; i++)
            {
                cumulativeDistances[i] = cumulativeDistances[i - 1] +
                                         HaversineDistance(route[i - 1].Latitude, route[i - 1].Longitude, route[i].Latitude, route[i].Longitude);
            }

            return cumulativeDistances;
        }

        private List<ActivitySplit> ComputeActivitySplits(
      List<GeoPoint> route,
      List<ActivityHr>? hrData,
      List<ActivitySpeed>? speedData,
      List<ActivityElevation>? elevationData,
      double[] cumulativeDistances,
      string measurementType)
        {
            // Work on a copy so original route isn't mutated
            var localRoute = route.ToList();

            var splits = new List<ActivitySplit>();
            double splitDistance = 1000; // meters
            int currentSplit = 1;

            int lastIndex = 0;
            double distanceSinceLastSplit = 0;

            for (int i = 1; i < localRoute.Count; i++)
            {
                double segmentDistance = cumulativeDistances[i] - cumulativeDistances[i - 1];
                distanceSinceLastSplit += segmentDistance;

                if (distanceSinceLastSplit >= splitDistance)
                {
                    double overshoot = distanceSinceLastSplit - splitDistance;
                    double needed = segmentDistance - overshoot;
                    double ratio = needed / segmentDistance;

                    GeoPoint splitPoint = InterpolatePoint(localRoute[i - 1], localRoute[i], ratio);

                    DateTime splitStartTime = localRoute[lastIndex].Timestamp;
                    DateTime splitEndTime = splitPoint.Timestamp;

                    double distanceMeters = splitDistance;
                    double durationSeconds = (splitEndTime - splitStartTime).TotalSeconds;

                    double avgSpeed = durationSeconds > 0 ? (distanceMeters / durationSeconds) : 0;

                    var hrSplit = hrData?
                        .Where(hr => hr.TimeStamp >= splitStartTime && hr.TimeStamp <= splitEndTime)
                        .Select(hr => hr.HearthRateValue)
                        .ToList();

                    var elevationSplit = elevationData?
                        .Where(e => e.Timestamp >= splitStartTime && e.Timestamp <= splitEndTime)
                        .ToList();

                    int? avgHr = hrSplit?.Count > 0 ? (int?)Math.Round(hrSplit.Average()) : null;

                    int? elevationGain = null;
                    if (elevationSplit != null && elevationSplit.Count > 1)
                    {
                        double gain = 0;
                        for (int j = 1; j < elevationSplit.Count; j++)
                        {
                            var delta = elevationSplit[j].ElevationValue - elevationSplit[j - 1].ElevationValue;
                            if (delta > 0) gain += delta;
                        }
                        elevationGain = (int)Math.Round(gain);
                    }

                    splits.Add(new ActivitySplit
                    {
                        SplitNumber = currentSplit++,
                        DistanceMeters = distanceMeters,
                        AvgSpeed = avgSpeed,
                        AvgHr = avgHr,
                        ElevationGain = elevationGain
                    });

                    // Insert into localRoute, NOT original route
                    localRoute.Insert(i, splitPoint);
                    cumulativeDistances = RecalculateCumulativeDistances(localRoute);

                    lastIndex = i;
                    distanceSinceLastSplit = 0;
                }
            }

            // After the loop, check for leftover distance and add a final split if > 0
            double leftoverDistance = cumulativeDistances.Last() - cumulativeDistances[lastIndex];
            if (leftoverDistance > 0)
            {
                DateTime splitStartTime = localRoute[lastIndex].Timestamp;
                DateTime splitEndTime = localRoute.Last().Timestamp;

                double distanceMeters = leftoverDistance;
                double durationSeconds = (splitEndTime - splitStartTime).TotalSeconds;

                double avgSpeed = durationSeconds > 0 ? (distanceMeters / durationSeconds) : 0;

                var hrSplit = hrData?
                    .Where(hr => hr.TimeStamp >= splitStartTime && hr.TimeStamp <= splitEndTime)
                    .Select(hr => hr.HearthRateValue)
                    .ToList();

                var elevationSplit = elevationData?
                    .Where(e => e.Timestamp >= splitStartTime && e.Timestamp <= splitEndTime)
                    .ToList();

                int? avgHr = hrSplit?.Count > 0 ? (int?)Math.Round(hrSplit.Average()) : null;

                int? elevationGain = null;
                if (elevationSplit != null && elevationSplit.Count > 1)
                {
                    double gain = 0;
                    for (int j = 1; j < elevationSplit.Count; j++)
                    {
                        var delta = elevationSplit[j].ElevationValue - elevationSplit[j - 1].ElevationValue;
                        if (delta > 0) gain += delta;
                    }
                    elevationGain = (int)Math.Round(gain);
                }

                splits.Add(new ActivitySplit
                {
                    SplitNumber = currentSplit++,
                    DistanceMeters = distanceMeters,
                    AvgSpeed = avgSpeed,
                    AvgHr = avgHr,
                    ElevationGain = elevationGain
                });
            }

            return splits;
        }

        private GeoPoint InterpolatePoint(GeoPoint start, GeoPoint end, double ratio)
        {
            return new GeoPoint
            {
                Latitude = start.Latitude + (end.Latitude - start.Latitude) * ratio,
                Longitude = start.Longitude + (end.Longitude - start.Longitude) * ratio,
                Timestamp = start.Timestamp + TimeSpan.FromTicks((long)((end.Timestamp - start.Timestamp).Ticks * ratio))
            };
        }

        // Helper to recalc cumulative distances after insertion
        private double[] RecalculateCumulativeDistances(List<GeoPoint> route)
        {
            double[] cumulativeDistances = new double[route.Count];
            cumulativeDistances[0] = 0;

            for (int i = 1; i < route.Count; i++)
            {
                cumulativeDistances[i] = cumulativeDistances[i - 1] + DistanceBetween(route[i - 1], route[i]);
            }

            return cumulativeDistances;
        }

        // Assuming you have a method like this for distance in meters between two GPS points
        private double DistanceBetween(GeoPoint p1, GeoPoint p2)
        {
            // Use Haversine formula or any distance calculation between lat/lon points
            var R = 6371000; // Radius of Earth in meters
            var lat1 = ToRadians(p1.Latitude);
            var lat2 = ToRadians(p2.Latitude);
            var deltaLat = ToRadians(p2.Latitude - p1.Latitude);
            var deltaLon = ToRadians(p2.Longitude - p1.Longitude);

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;

        private bool CheckIfActivityHasNewRecord(int activityId)
        {
            // Fetch segment efforts for this activity
            var thisActivityEfforts = _context.BestEfforts
                .Where(se => se.UserActivityId == activityId)
                .ToList();

            foreach (var effort in thisActivityEfforts)
            {
                // Find the fastest time recorded for this segment among all other activities
                var bestTime = _context.BestEfforts
                    .Where(se => se.SegmentConfigId == effort.SegmentConfigId && se.UserActivityId != activityId)
                    .OrderBy(se => se.DurationSeconds)
                    .Select(se => se.DurationSeconds)
                    .FirstOrDefault();

                // If no previous record or this activity's effort is faster => new record!
                if (bestTime == 0 || effort.DurationSeconds < bestTime)
                {
                    return true;
                }
            }

            return false;
        }
    }
}