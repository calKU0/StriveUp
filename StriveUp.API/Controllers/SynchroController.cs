using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using System.Collections.Generic;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SynchroController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SynchroController(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("availableProviders")]
        public async Task<ActionResult<List<UserSynchroDto>>> GetAvailableProviders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var allProviders = await _context.SynchroProviders.ToListAsync();

                var connectedProviderIds = await _context.UserSynchros
                    .Where(us => us.UserId == userId)
                    .Select(us => us.SynchroId)
                    .ToListAsync();

                var availableProviders = allProviders
                    .Where(p => !connectedProviderIds.Contains(p.Id))
                    .ToList();

                var dtos = _mapper.Map<List<UserSynchroDto>>(availableProviders);

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("userSynchros")]
        public async Task<ActionResult<UserSynchroDto>> GetUserSynchros()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var providers = await _context.UserSynchros
                    .Include(s => s.SynchroProvider)
                    .Where(s => s.UserId == userId)
                    .ToListAsync();

                var dtos = _mapper.Map<List<UserSynchroDto>>(providers);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("allUsersSynchros")]
        public async Task<ActionResult<UserSynchroDto>> GetAllUserSynchros()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users
                    .Include(u => u.Level)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (userId == null) return Unauthorized();

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    return Forbid();
                }

                var providers = await _context.UserSynchros
                .Include(s => s.SynchroProvider)
                .ToListAsync();

                var dtos = _mapper.Map<List<UserSynchroDto>>(providers);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("addUserSynchro")]
        public async Task<ActionResult<UserSynchroDto>> AddUserSynchro([FromBody] CreateUserSynchroDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var userSynchro = _mapper.Map<UserSynchro>(dto);
                userSynchro.UserId = userId;

                _context.UserSynchros.Add(userSynchro);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<UserSynchroDto>(userSynchro);

                return CreatedAtAction(nameof(GetUserSynchros), new { id = userSynchro.Id }, resultDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("updateUserSynchro/{id}")]
        public async Task<IActionResult> UpdateUserSynchro(int id, [FromBody] UpdateUserSynchroDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var userSynchro = await _context.UserSynchros
                    .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

                if (userSynchro == null)
                    return NotFound("UserSynchro not found");

                _mapper.Map(dto, userSynchro);
                userSynchro.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("deleteUserSynchro/{id}")]
        public async Task<IActionResult> DeleteUserSynchro(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var synchro = await _context.UserSynchros
                    .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

                if (synchro == null)
                    return NotFound("UserSynchro not found");

                _context.UserSynchros.Remove(synchro);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
