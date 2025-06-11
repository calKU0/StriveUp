using Blazored.LocalStorage;
using StriveUp.Infrastructure.Extensions;
using StriveUp.Shared.Interfaces;
using StriveUp.Web.Components;
using StriveUp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the StriveUp.Shared project
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPlatformService, WebPlatformService>();
builder.Services.AddScoped<IAppSettingsService, AppSettingsService>();
builder.Services.AddClientInfrastructure(builder.Configuration);
builder.Services.AddHttpClient("ApiClient", (client) =>
{
    client.BaseAddress = new Uri("https://localhost:7116/api/");
});

//builder.WebHost.ConfigureKestrel(options =>
//{
//    var certPath = @"C:\certyfikaty\cert.pfx";
//    var certPassword = "";

//    options.Listen(IPAddress.Any, 443, listenOptions =>
//    {
//        listenOptions.UseHttps(certPath, certPassword);
//    });

//    // (opcjonalnie) nas³uchiwanie HTTP i przekierowanie do HTTPS
//    options.Listen(IPAddress.Any, 80);
//});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(StriveUp.Shared._Imports).Assembly);

app.UseStaticFiles();

// Handle exception - Not Found
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled exception: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal Server Error");
    }
});

// Handle 404 - Not Found
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        Console.WriteLine($"404 Not Found: {context.Request.Path}");

        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("404 Not Found");
    }
});

app.Run();