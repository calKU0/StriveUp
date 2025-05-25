using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using StriveUp.API.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginRequest = StriveUp.Shared.DTOs.LoginRequest;
using RegisterRequest = StriveUp.Shared.DTOs.RegisterRequest;

public class AuthService : StriveUp.API.Interfaces.IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IImageService _imageService;
    private readonly AppDbContext _context;

    public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration configuration, IImageService imageService, AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _imageService = imageService;
        _context = context;
    }

    public async Task<(bool Success, JwtResponse Token)> LoginAsync(LoginRequest request)
    {
        AppUser user = null;

        if (IsValidEmail(request.Login))
        {
            user = await _userManager.FindByEmailAsync(request.Login);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request.Login);
        }

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return (false, null);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(31);

        await _userManager.UpdateAsync(user);

        return (true, new JwtResponse { Token = token, RefreshToken = refreshToken });
    }

    public async Task<(IdentityResult Result, JwtResponse Token)> RegisterAsync(RegisterRequest request)
    {
        try
        {
            string avatarUrl = null;

            if (request.Password != request.RepeatPassword)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." }), null);
            }

            if (!string.IsNullOrEmpty(request.AvatarBase64))
            {
                var bytes = Convert.FromBase64String(request.AvatarBase64);
                var stream = new MemoryStream(bytes);
                stream.Position = 0;

                var file = new FormFile(stream, 0, bytes.Length, "avatar", "avatar.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
                avatarUrl = await _imageService.UploadImageAsync(file);
            }

            var user = new AppUser
            {
                UserName = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Avatar = avatarUrl ?? "images/icons/user.png",
                Gender = request.Gender,
                Birthday = request.Birthday,
                Country = request.Country,
                State = request.State,
                City = request.City,
                Bio = request.Bio
            };

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(31);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return (result, null);

            var config = new UserConfig
            {
                UserId = user.Id
            };

            _context.UserConfigs.Add(config);
            await _context.SaveChangesAsync();

            return (result, new JwtResponse { Token = token, RefreshToken = refreshToken });
        }
        catch (FormatException ex)
        {
            return (IdentityResult.Failed(new IdentityError { Description = "Invalid avatar image format." }), null);
        }
        catch (Exception ex)
        {
            return (IdentityResult.Failed(new IdentityError { Description = $"Unexpected error: {ex.Message}" }), null);
        }
    }

    public async Task<(bool Success, JwtResponse Token)> ExternalLoginAsync(ClaimsPrincipal externalUser)
    {
        try
        {
            var email = externalUser.FindFirstValue(ClaimTypes.Email);
            if (email == null) return (false, null);

            var user = await _userManager.FindByEmailAsync(email);

            // Auto-register new external user
            if (user == null)
            {
                var userName = email.Split('@')[0];
                user = new AppUser
                {
                    UserName = userName,
                    Email = email,
                    FirstName = externalUser.FindFirstValue(ClaimTypes.GivenName) ?? userName,
                    LastName = externalUser.FindFirstValue(ClaimTypes.Surname) ?? "",
                };
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(31);

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return (false, null);

                var config = new UserConfig
                {
                    UserId = user.Id
                };

                _context.UserConfigs.Add(config);
                await _context.SaveChangesAsync();

                return (true, new JwtResponse { Token = token, RefreshToken = refreshToken });
            }
        }
        catch
        {
        }
        return (false, null);
    }

    public async Task DeleteAccountAsync(string userId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            // Delete BestEfforts -> tied to UserActivities
            var bestEfforts = await _context.BestEfforts
                .Where(be => be.User.Id == userId)
                .ToListAsync();
            _context.BestEfforts.RemoveRange(bestEfforts);

            // Delete ActivityComments & Likes (Restrict FK)
            var userActivityIds = await _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.Id)
                .ToListAsync();

            var comments = await _context.ActivityComments
                .Where(c => userActivityIds.Contains(c.UserActivityId))
                .ToListAsync();
            _context.ActivityComments.RemoveRange(comments);

            var likes = await _context.ActivityLikes
                .Where(l => userActivityIds.Contains(l.UserActivityId))
                .ToListAsync();
            _context.ActivityLikes.RemoveRange(likes);

            // Delete UserFollowers
            var followers = await _context.UserFollowers
                .Where(f => f.FollowerId == userId || f.FollowedId == userId)
                .ToListAsync();
            _context.UserFollowers.RemoveRange(followers);

            // Notifications where user is Actor (Restrict FK)
            var notificationsAsActor = await _context.Notifications
                .Where(n => n.ActorId == userId)
                .ToListAsync();
            _context.Notifications.RemoveRange(notificationsAsActor);

            // Notifications owned by user (Cascade)
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();
            _context.Notifications.RemoveRange(notifications);

            // MedalsEarned (Cascade)
            var medals = await _context.MedalsEarned
                .Where(me => me.UserId == userId)
                .ToListAsync();
            _context.MedalsEarned.RemoveRange(medals);

            // UserSynchros
            var synchros = await _context.UserSynchros
                .Where(us => us.UserId == userId)
                .ToListAsync();
            _context.UserSynchros.RemoveRange(synchros);

            // UserActivities (Cascade)
            var activities = await _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .ToListAsync();
            _context.UserActivities.RemoveRange(activities);

            // UserConfig
            var config = await _context.UserConfigs
                .FirstOrDefaultAsync(uc => uc.UserId == userId);
            if (config != null)
                _context.UserConfigs.Remove(config);

            // Delete the user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<JwtResponse> RefreshToken(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity?.Name;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new JwtResponse { Token = newToken, RefreshToken = newRefreshToken };
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
            ValidateLifetime = false,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.ValidateToken(token, tokenValidationParameters, out _);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string GenerateJwtToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool IsValidEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(input);
            return addr.Address == input;
        }
        catch
        {
            return false;
        }
    }
}