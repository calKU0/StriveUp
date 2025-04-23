using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using StriveUp.API.Services;
using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using LoginRequest = StriveUp.Shared.DTOs.LoginRequest;
using RegisterRequest = StriveUp.Shared.DTOs.RegisterRequest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StriveUp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IImageService _imageService;

    public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration configuration, IImageService imageService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _imageService = imageService;
    }

    public async Task<(bool Success, string Token)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return (false, null);

        var token = await GenerateJwtToken(user);
        return (true, token);
    }

    public async Task<(IdentityResult Result, string Token)> RegisterAsync(RegisterRequest request)
    {
        string avatarUrl = null;

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
            Avatar = avatarUrl,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return (result, null);

        var token = await GenerateJwtToken(user);
        return (result, token);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    private async Task<string> GenerateJwtToken(AppUser user)
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
}
