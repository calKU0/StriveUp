using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Plugin.BLE.Abstractions.Contracts;
using StriveUp.API.Mapping;
using StriveUp.API.Services;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Data.Settings;
using StriveUp.Infrastructure.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication()
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Token validation failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
})
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["GoogleSettings:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleSettings:ClientSecret"];
    options.Scope.Add("email");        // Add this
    options.Scope.Add("profile");
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<ISecurableService, SecurableService>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var services = scope.ServiceProvider;

    await Seed.SeedAdminRoleAndUser(services);
    await Seed.SeedData(context);
}

// Middlewares
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
