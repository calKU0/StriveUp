using StriveUp.Web.Components;
using StriveUp.Shared.Interfaces;
using StriveUp.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using StriveUp.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the StriveUp.Shared project
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPlatformService, WebPlatformService>();
builder.Services.AddClientInfrastructure();
builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7116/api/");
});


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
app.Run();
