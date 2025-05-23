using Microsoft.EntityFrameworkCore;
using StriveUp.API.Interfaces;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Leaderboard;
using StriveUp.Shared.Helpers;
using System.Security.Claims;

namespace StriveUp.API.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly AppDbContext _context;

        public LeaderboardService(AppDbContext context)
        {
            _context = context;
        }

        // Get the best efforts for a particular distance (5km, 10km, etc.) and activity type (running, cycling, etc.)
        public async Task<List<LeaderboardDto>> GetBestFollowersEfforts(string userId, int activityId, int distance)
        {
            List<string> userIds = await GetUserAndFollowersIdsAsync(userId);

            var query = _context.BestEfforts
                .Include(be => be.SegmentConfig)
                .Include(be => be.User)
                .Include(be => be.UserActivity)
                .ThenInclude(ua => ua.Activity)
                .Where(be => userIds.Contains(be.UserId)
                             && be.SegmentConfig.DistanceMeters == distance
                             && be.SegmentConfig.ActivityId == activityId);

            // Fetch data into memory first
            var allEfforts = await query.ToListAsync();

            // Then group and select on client side
            var bestEfforts = allEfforts
                .GroupBy(be => be.UserId)
                .Select(g => g.OrderBy(be => be.DurationSeconds).First())
                .Select(be => new LeaderboardDto
                {
                    UserId = be.UserId,
                    Username = be.User.UserName!,
                    UserAvatar = be.User.Avatar,
                    TotalDuration = be.DurationSeconds,
                    Distance = (int)be.SegmentConfig.DistanceMeters,
                    Speed = be.Speed,
                    ActivityDate = be.UserActivity.DateEnd,
                    ActivityId = be.UserActivityId
                })
                .ToList();

            return bestEfforts;
        }

        public async Task<List<DistanceLeaderboardDto>> GetFollowersDistanceLeaderboard(string userId, int activityId, string timeframe)
        {
            List<string> userIds = await GetUserAndFollowersIdsAsync(userId);

            var query = _context.UserActivities
                .Include(ua => ua.Activity)
                .Include(ua => ua.User)
                .Where(ua => userIds.Contains(ua.UserId) && ua.ActivityId == activityId);

            if (timeframe == "weekly")
            {
                var startOfWeek = DateUtils.GetStartOfWeek(DateTime.UtcNow);
                query = query.Where(ua => ua.DateEnd >= startOfWeek);
            }
            else if (timeframe == "monthly")
            {
                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                query = query.Where(ua => ua.DateEnd >= startOfMonth);
            }
            else if (timeframe == "yearly")
            {
                var startOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
                query = query.Where(ua => ua.DateEnd >= startOfYear);
            }
            else if (timeframe == "alltime")
            {
                // No additional filter needed for all time
            }
            else
            {
                throw new ArgumentException("Invalid timeframe specified.");
            }

            if (timeframe == "weekly")
            {
                var startOfWeek = DateUtils.GetStartOfWeek(DateTime.UtcNow);
                query = query.Where(ua => ua.DateEnd >= startOfWeek);
            }

            var activityGroups = await query
                .GroupBy(ua => ua.UserId)
                .Select(g => new DistanceLeaderboardDto
                {
                    UserId = g.Key,
                    Username = g.First().User.UserName!,
                    UserAvatar = g.First().User.Avatar,
                    TotalDistance = g.Sum(ua => ua.Distance) / 1000.0
                })
                .OrderByDescending(dto => dto.TotalDistance)
                .ToListAsync();

            return activityGroups;
        }

        public async Task<List<LevelLeaderboardDto>> GetFollowersLevelLeaderboard(string userId)
        {
            List<string> userIds = await GetUserAndFollowersIdsAsync(userId);

            var levels = await _context.Users
                .Include(u => u.Level)
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new LevelLeaderboardDto
                {
                    UserId = u.Id,
                    Username = u.UserName!,
                    UserAvatar = u.Avatar,
                    Level = u.Level.LevelNumber,
                    ExperiencePoints = u.CurrentXP
                })
                .OrderByDescending(l => l.Level)
                .ThenByDescending(l => l.ExperiencePoints)
                .ToListAsync();

            return levels;
        }

        private async Task<List<string>> GetUserAndFollowersIdsAsync(string userId)
        {
            var followers = await _context.UserFollowers
                .Where(uf => uf.FollowerId == userId)
                .Select(uf => uf.FollowedId)
                .ToListAsync();

            // Include the user themselves
            followers.Add(userId);

            return followers;
        }
    }
}
