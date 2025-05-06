using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Data;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

namespace StriveUp.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MedalController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MedalController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("medals")]
        public async Task<ActionResult<IEnumerable<MedalDto>>> GetAllMedals()
        {
            try
            {
                var medals = await _context.Medals
                    .ProjectTo<MedalDto>(_mapper.ConfigurationProvider) 
                    .ToListAsync();

                return Ok(medals);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("userMedals")]
        public async Task<ActionResult<IEnumerable<MedalDto>>> GetUserMedals()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var medals = await _context.MedalsEarned
                    .Where(m => m.UserId == userId)
                    .Include(m => m.Medal)
                    .ToListAsync();

                var medalDtos = _mapper.Map<List<MedalDto>>(medals);

                return Ok(medalDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
