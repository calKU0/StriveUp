using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StriveUp.API.Services;
using StriveUp.Shared.DTOs;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var (success, token) = await _authService.LoginAsync(request);
            return success ? Ok(new JwtResponse { Token = token }) : Unauthorized("Invalid credentials.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var (result, token) = await _authService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Message = "Registration failed.", Errors = errors });
            }

            return Ok(new JwtResponse { Token = token });
        }
    }
}
