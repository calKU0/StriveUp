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
using StriveUp.Shared.DTOs.Profile;
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
        private readonly INotificationService _notificationService;

        public FollowController(AppDbContext context, UserManager<AppUser> userManager, IMapper mapper, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        [HttpGet("followers/{userName}")]
        public async Task<ActionResult<List<UserFollowDto>>> GetUserFollowers(string userName)
        {
            try
            {
                var user = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
                if (user is null)
                {
                    return BadRequest("No user with such nickname found.");
                }

                var userId = user.Id;
                // Get IDs the current user is following
                var followingIds = await _context.UserFollowers
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FollowedId)
                    .ToListAsync();

                var followingSet = new HashSet<string>(followingIds);

                // Get followers and map to DTO
                var followers = await _context.UserFollowers
                    .Where(uf => uf.FollowedId == userId)
                    .Select(uf => new UserFollowDto
                    {
                        UserId = uf.Follower.Id,
                        FullName = uf.Follower.FirstName + " " + uf.Follower.LastName,
                        UserName = uf.Follower.UserName!,
                        Avatar = uf.Follower.Avatar,
                        IsFollowed = followingSet.Contains(uf.FollowerId)
                    })
                    .ToListAsync();

                return Ok(followers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("following/{userName}")]
        public async Task<ActionResult<List<UserFollowDto>>> GetUserFollowing(string userName)
        {
            try
            {
                var user = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
                if (user is null)
                {
                    return BadRequest("No user with such nickname found.");
                }

                var userId = user.Id;

                // Get IDs of users who follow the current user (to check mutual follows)
                var followersOfCurrentUser = await _context.UserFollowers
                    .Where(f => f.FollowedId == userId)
                    .Select(f => f.FollowerId)
                    .ToListAsync();

                var followersSet = new HashSet<string>(followersOfCurrentUser);

                // Get users the current user is following
                var following = await _context.UserFollowers
                    .Where(f => f.FollowerId == userId)
                    .Select(f => new UserFollowDto
                    {
                        UserId = f.Followed.Id,
                        FullName = f.Followed.FirstName + " " + f.Followed.LastName,
                        UserName = f.Followed.UserName!,
                        Avatar = f.Followed.Avatar,
                        IsFollowed = followersSet.Contains(f.FollowedId)
                    })
                    .ToListAsync();

                return Ok(following);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserFollowDto>>> SearchUsers(string keyword)
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
                                (u.UserName!.Contains(keyword) ||
                                 u.FirstName.Contains(keyword) ||
                                 u.LastName.Contains(keyword)))
                    .Take(20)
                    .ToListAsync();

                var results = _mapper.Map<List<UserFollowDto>>(users);
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

        [HttpPost("{userId}")]
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

                    // Fetch follower user info to personalize notification
                    var followerUser = await _context.Users
                        .Where(u => u.Id == currentUserId)
                        .Select(u => new { u.UserName })
                        .FirstOrDefaultAsync();

                    if (followerUser != null)
                    {
                        var notifDto = new CreateNotificationDto
                        {
                            UserId = userId,          // The followed user (recipient)
                            ActorId = currentUserId,  // The follower (actor)
                            Title = "New Follower",
                            Message = $"{followerUser.UserName} started following you.",
                            Type = "follow",
                            RedirectUrl = $"/profile/{followerUser.UserName}"
                        };

                        await _notificationService.CreateNotificationAsync(notifDto);
                    }

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

        [HttpDelete("{userId}")]
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