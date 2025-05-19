using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StriveUp.API.Services;
using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.DTOs;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(IAuthService authService, SignInManager<AppUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var (success, token) = await _authService.LoginAsync(request);
            return success ? Ok(token) : Unauthorized("Invalid credentials.");
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

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var token = await _authService.RefreshToken(request);
            if(token == null)
                return Unauthorized("Invalid refresh token.");

            return Ok(token);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "https://localhost:7153/login")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl })
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (info?.Principal == null)
            {
                return Unauthorized("External login info not found.");
            }

            var(success, token) = await _authService.ExternalLoginAsync(info.Principal);
            if (!success)
                return Unauthorized("External login failed.");

            // Redirect to client with token (you may also issue a Set-Cookie here)
            return Redirect($"{returnUrl}?token={token}");
        }

    }
}
