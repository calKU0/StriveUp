using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;

namespace StriveUp.API.Services
{
    public class LevelService : ILevelService
    {
        private readonly AppDbContext _context;

        public LevelService(AppDbContext context)
        {
            _context = context;
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
            }
        }

    }
}
