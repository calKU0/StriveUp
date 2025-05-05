using Microsoft.Extensions.Logging;
using StriveUp.Shared.Interfaces;
using StriveUp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using StriveUp.Web.Services;

namespace StriveUp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add device-specific services used by the StriveUp.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<CustomAuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
        builder.Services.AddScoped<ICustomAuthStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<IMedalsService, MedalService>();
        builder.Services.AddSingleton<IPlatformService, MauiPlatformService>();

        var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7116);
        var http = devSslHelper.HttpClient;
        http.BaseAddress = new Uri(devSslHelper.DevServerRootUrl + "/api/");

        builder.Services.AddSingleton(sp => http);

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
