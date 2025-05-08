using Microsoft.Extensions.Logging;
using StriveUp.Shared.Interfaces;
using StriveUp.MAUI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using StriveUp.Infrastructure.Extensions;

namespace StriveUp.MAUI;

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
        builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<IPlatformService, MauiPlatformService>();
        builder.Services.AddClientInfrastructure();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddHttpClient("ApiClient", (sp, client) =>
        {
            client.BaseAddress = new Uri("https://striveupapi-emaee9awang6g4ht.polandcentral-01.azurewebsites.net" + "/api/");
        });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
