using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public NotificationsController(AppDbContext context, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetUserNotifications()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var notifs = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(20)
                    .ToListAsync();

                var notifDtos = _mapper.Map<List<NotificationDto>>(notifs);

                // Enrich with actor names
                var actorIds = notifDtos.Select(n => n.ActorId).Distinct().ToList();
                var users = await _userManager.Users
                    .Where(u => actorIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Avatar
                    })
                    .ToDictionaryAsync(u => u.Id);

                foreach (var dto in notifDtos)
                {
                    if (users.TryGetValue(dto.ActorId, out var user))
                    {
                        dto.ActorAvatar = user.Avatar;

                        if (dto.ActorId != userId)
                        {
                            dto.ActorName = user.UserName;
                        }
                    }
                }

                return Ok(notifDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (notif == null) return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("readAll")]
        public async Task<IActionResult> MarkAsReadAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var notifs = await _context.Notifications.Where(n => n.IsRead == false && n.UserId == userId).ToListAsync();
            if (notifs == null) return NotFound();

            foreach (var notif in notifs)
            {
                notif.IsRead = true;
            }
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}