using Microsoft.EntityFrameworkCore;
using StriveUp.API.Interfaces;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs;
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

        public async Task<List<LeaderboardDto>> GetTopTimeSpentAsync(string userId)
        {
            List<string> userIds = await GetUserAndFollowersIdsAsync(userId);

            return await _context.UserActivities
                .Where(ua => userIds.Contains(ua.UserId))
                .GroupBy(ua => ua.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalDuration = g.Sum(ua => ua.DurationSeconds)
                })
                .OrderByDescending(u => u.TotalDuration)
                .Take(10)
                .Join(_context.Users, u => u.UserId, user => user.Id, (u, user) => new LeaderboardDto
                {
                    UserId = u.UserId,
                    Username = user.UserName,
                    TotalDuration = u.TotalDuration
                })
                .ToListAsync();
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
