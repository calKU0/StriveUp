using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using StriveUp.API.Services;
using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using LoginRequest = StriveUp.Shared.DTOs.LoginRequest;
using RegisterRequest = StriveUp.Shared.DTOs.RegisterRequest;
using Microsoft.AspNetCore.Authentication;

public class AuthService : IAuthService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterRequest request)
    {
        var user = new AppUser
        {
            UserName = request.Username,
            Email = request.Email,
            LastName = request.LastName,
            FirstName = request.FirstName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        return result;
    }

    public async Task<bool> LoginAsync(LoginRequest request, HttpContext httpContext)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) return false;

        var isCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isCorrect) return false;

        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

        return true;
    }
}
