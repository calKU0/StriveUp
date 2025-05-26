using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StriveUp.API.Interfaces;
using StriveUp.Shared.DTOs;
using System.Security.Claims;

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
            try
            {
                var (success, token) = await _authService.LoginAsync(request);
                return success
                    ? Ok(token)
                    : Unauthorized(new ErrorResponse { Message = "Invalid credentials." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = "An error occurred during login." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var (result, token) = await _authService.RegisterAsync(request);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { Message = "Registration failed.", Errors = errors });
                }

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = "An error occurred during login." });
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "User not authenticated." });
                }

                await _authService.DeleteAccountAsync(userId);
                return NoContent(); // 204 No Content on success
            }
            catch (InvalidOperationException ex)
            {
                // User not found or other expected errors
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Unexpected errors
                return StatusCode(500, new { message = "An error occurred while deleting the account.", detail = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var token = await _authService.RefreshToken(request);
            if (token == null)
                return Unauthorized(new ErrorResponse { Message = "Invalid refresh token." });

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
                return Unauthorized(new { Message = "External login info not found." });

            var (success, jwt) = await _authService.ExternalLoginAsync(info.Principal);
            if (!success || jwt == null)
                return Unauthorized(new { Message = "External login failed." });

            if (returnUrl.StartsWith("striveup://"))
            {
                // For native apps, pass tokens in query string, NOT fragment
                var redirectUrl = $"{returnUrl}?access_token={jwt.Token}&refresh_token={jwt.RefreshToken}";
                return Redirect(redirectUrl);
            }
            else
            {
                // For web, keep tokens in fragment as before
                var redirectUrl = $"{returnUrl}#access_token={jwt.Token}&refresh_token={jwt.RefreshToken}";
                return Redirect(redirectUrl);
            }
        }
    }
}