using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class FollowController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FollowController(AppDbContext context, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<FollowDto>>> SearchUsers(string keyword)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized();

                var currentUser = await _context.Users
                    .Include(u => u.Following)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId);
                var followedIds = currentUser?.Following.Select(f => f.FollowedId).ToHashSet() ?? new();

                var users = await _context.Users
                    .Where(u => u.Id != currentUserId &&
                                (u.UserName.Contains(keyword) ||
                                 u.FirstName.Contains(keyword) ||
                                 u.LastName.Contains(keyword)))
                    .Take(20)
                    .ToListAsync();

                var results = _mapper.Map<List<FollowDto>>(users);
                foreach (var dto in results)
                {
                    dto.IsFollowed = followedIds.Contains(dto.UserId);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{userId}/follow")]
        public async Task<IActionResult> FollowUser(string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

                if (currentUserId == userId) return BadRequest("Cannot follow yourself.");

                var exists = await _context.UserFollowers
                    .AnyAsync(f => f.FollowerId == currentUserId && f.FollowedId == userId);

                if (!exists)
                {
                    _context.UserFollowers.Add(new UserFollower
                    {
                        FollowerId = currentUserId,
                        FollowedId = userId
                    });

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}/unfollow")]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            try
            { 
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

                var follow = await _context.UserFollowers
                    .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == userId);

                if (follow != null)
                {
                    _context.UserFollowers.Remove(follow);
                    await _context.SaveChangesAsync();
                }

            return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
