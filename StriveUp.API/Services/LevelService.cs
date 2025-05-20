using Microsoft.EntityFrameworkCore;
using StriveUp.API.Interfaces;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.DTOs;

namespace StriveUp.API.Services
{
    public class LevelService : ILevelService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public LevelService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task UpdateUserLevelAsync(AppUser user)
        {
            // Only fetch levels once
            var levels = await _context.Levels
                .AsNoTracking()
                .OrderBy(l => l.TotalXP)
                .ToListAsync();

            var newLevel = levels.FirstOrDefault(l => l.TotalXP >= user.CurrentXP);

            if (newLevel != null && user.LevelId != newLevel.Id)
            {
                user.LevelId = newLevel.Id;
                var notifDto = new CreateNotificationDto
                {
                    UserId = user.Id,
                    ActorId = user.Id, // user is actor of their own level up
                    Title = "Level Up!",
                    Message = $"Congratulations! You've reached level {newLevel.LevelNumber}.",
                    Type = "levelup",
                    RedirectUrl = "/profile/levels" // or wherever appropriate
                };
                await _notificationService.CreateNotificationAsync(notifDto);
            }
        }

    }
}
