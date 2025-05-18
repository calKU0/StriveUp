using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StriveUp.Infrastructure.Data;
using StriveUp.Infrastructure.Data.Settings;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Services;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnectionString")));


            services.AddScoped<IImageService, ImageService>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.Configure<MapboxSettings>(config.GetSection("MapboxSettings"));
            services.Configure<GoogleSettings>(config.GetSection("GoogleSettings"));
            services.Configure<FitbitSettings>(config.GetSection("FitbitSettings"));

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_+";
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddClientInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<GoogleSettings>(config.GetSection("GoogleSettings"));
            services.Configure<FitbitSettings>(config.GetSection("FitbitSettings"));
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IMedalService, MedalService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ISecurableService, SecurableService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IMedalStateService, MedalStateService>();
            services.AddScoped<ISynchroService, SynchroService>();
            services.AddScoped<CustomAuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
            services.AddScoped<ICustomAuthStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

            return services;
        }
    }
}
