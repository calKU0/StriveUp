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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return BadRequest($"Rejestracja nie powiodła się: {errors}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request, HttpContext);
            return result ? Ok() : Unauthorized("Nieprawidłowe dane logowania.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok();
        }

    }
}
