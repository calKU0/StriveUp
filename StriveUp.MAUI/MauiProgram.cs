using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shiny;
using StriveUp.Infrastructure.Extensions;
using StriveUp.MAUI.Services;
using StriveUp.Shared.Interfaces;
using System.Diagnostics;
using System.Reflection;

#if ANDROID

using StriveUp.MAUI.Platforms.Android;

#endif

namespace StriveUp.MAUI;

public static class MauiProgram
{
    public static class MauiServiceProvider
    {
        public static IServiceProvider Services { get; set; }
    }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream("StriveUp.MAUI.appsettings.json");

        var configBuilder = new ConfigurationBuilder();

        if (stream != null)
        {
            configBuilder.AddJsonStream(stream);
        }
        else
        {
            Debug.WriteLine("appsettings.json nie został znaleziony jako zasób");
        }

        var configuration = configBuilder.Build();

        builder
            .UseMauiApp<App>()
            //.UseShiny()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add device-specific services used by the StriveUp.Shared project
        builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddSingleton<IPlatformService, MauiPlatformService>();
        builder.Services.AddSingleton<IBleHeartRateService, BleHeartRateService>();
        builder.Services.AddSingleton<IAppStateService, AppStateService>();
#if ANDROID
        builder.Services.AddSingleton<IActivityTrackingService, ActivityTrackingService>();
        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
#endif
        builder.Services.AddClientInfrastructure(builder.Configuration);
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

        var app = builder.Build();
        MauiServiceProvider.Services = app.Services;
        return app;
    }
}