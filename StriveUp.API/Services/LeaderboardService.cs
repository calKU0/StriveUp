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
        public async Task<List<LeaderboardDto>> GetTopDistanceAsync(string userId, string activityType, int distance)
        {
            List<string> userIds = await GetUserAndFollowersIdsAsync(userId);

            return await _context.UserActivities
                .Where(ua => userIds.Contains(ua.UserId) && ua.Activity.Name == activityType && ua.Distance >= distance)
                .OrderByDescending(ua => ua.Distance)
                .Take(10)
                .Select(ua => new LeaderboardDto
                {
                    UserId = ua.UserId,
                    Username = ua.User.UserName,
                    Distance = ua.Distance,
                    DateStart = ua.DateStart,
                    ActivityType = ua.Activity.Name
                })
                .ToListAsync();
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
