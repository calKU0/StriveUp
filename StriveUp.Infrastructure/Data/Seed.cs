using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;

namespace StriveUp.Infrastructure.Data
{
    public static class Seed
    {
        public static async Task SeedAdminRoleAndUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            const string adminRole = "Admin";
            const string adminEmail = "krzysztop51@wp.pl";

            // 1. Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // 2. Find your user
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                Console.WriteLine("Admin user not found.");
                return;
            }

            // 3. Assign user to Admin role
            if (!await userManager.IsInRoleAsync(user, adminRole))
            {
                await userManager.AddToRoleAsync(user, adminRole);
                Console.WriteLine("Admin role assigned.");
            }
            else
            {
                Console.WriteLine("User is already an admin.");
            }
        }

        public static async Task SeedData(AppDbContext context)
        {
            if (!context.SynchroProviders.Any())
            {
                var providers = new List<SynchroProvider>
                {
                    new SynchroProvider
                    {
                        Name = "Google Fit",
                        IconUrl = "/images/icons/google-fit-icon.png",
                        IsActive = false,
                    },
                    new SynchroProvider
                    {
                        Name = "Fitbit",
                        IconUrl = "/images/icons/fitbit-icon.png",
                        IsActive = true,
                    },
                    new SynchroProvider
                    {
                        Name = "Garmin Connect",
                        IconUrl = "/images/icons/garmin-icon.png",
                        IsActive = false,
                    },
                    new SynchroProvider
                    {
                        Name = "Strava",
                        IconUrl = "/images/icons/strava-icon.png",
                        IsActive = false,
                    }
                };

                context.SynchroProviders.AddRange(providers);
                await context.SaveChangesAsync();
            }

            if (!context.Activities.Any())
            {
                var activities = new List<Activity>
                {
                    new Activity { Name = "Run", Description = "Running", AverageCaloriesPerHour = 590 },
                    new Activity { Name = "Treadmill", Description = "Indoor treadmill running or walking", AverageCaloriesPerHour = 500 },
                    new Activity { Name = "Walk", Description = "Brisk walking", AverageCaloriesPerHour = 280 },
                    new Activity { Name = "Bike", Description = "Cycling", AverageCaloriesPerHour = 720 },
                    new Activity { Name = "Hike", Description = "Hiking", AverageCaloriesPerHour = 360 },
                    new Activity { Name = "Swim", Description = "Swimming", AverageCaloriesPerHour = 593 },
                    new Activity { Name = "Elliptical", Description = "Elliptical trainer", AverageCaloriesPerHour = 400 },
                    new Activity { Name = "Jump Rope", Description = "Jumping rope", AverageCaloriesPerHour = 1020 },
                    new Activity { Name = "Weight Training", Description = "Weight lifting", AverageCaloriesPerHour = 315 },
                    new Activity { Name = "Yoga", Description = "Yoga", AverageCaloriesPerHour = 320 },
                    new Activity { Name = "Dancing", Description = "Dancing", AverageCaloriesPerHour = 450 },
                    new Activity { Name = "Other", Description = "Other not specified activity", AverageCaloriesPerHour = 350 }
                };

                context.Activities.AddRange(activities);
                await context.SaveChangesAsync();
            }

            var runningActivity = context.Activities.First(a => a.Name == "Run");
            //var treadmillActivity = context.Activities.First(a => a.Name == "Treadmill");
            //var walkingActivity = context.Activities.First(a => a.Name == "Walk");
            var bikingActivity = context.Activities.First(a => a.Name == "Bike");
            //var hikingActivity = context.Activities.First(a => a.Name == "Hike");
            var swimmingActivity = context.Activities.First(a => a.Name == "Swim");
            //var ellipticalActivity = context.Activities.First(a => a.Name == "Elliptical");
            //var jumpropeActivity = context.Activities.First(a => a.Name == "Jump Rope");
            //var weightTrainingActivity = context.Activities.First(a => a.Name == "Weight Training");
            //var yogaActivity = context.Activities.First(a => a.Name == "Yoga");
            //var dancingActivity = context.Activities.First(a => a.Name == "Dancing");
            //var otherActivity = context.Activities.First(a => a.Name == "Other");


            //if (!context.ActivityConfig.Any())
            //{
            //    var configs = new List<ActivityConfig>
            //    {
            //        new ActivityConfig
            //        {
            //            ActivityId = runningActivity.Id,
            //            MeasurementType = "pace",
            //            DefaultDistanceUnit = "km",
            //            UseHeartRate = true,
            //            ElevationRelevant = true,
            //            DistanceRelevant = true,
            //            SpeedRelevant = true,
            //            Indoor = false,
            //            PointsPerMinute = 2
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = treadmillActivity.Id,
            //            MeasurementType = "pace",
            //            DefaultDistanceUnit = "km",
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = true,
            //            SpeedRelevant = true,
            //            Indoor = true,
            //            PointsPerMinute = 2
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = walkingActivity.Id,
            //            MeasurementType = "pace",
            //            DefaultDistanceUnit = "km",
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = true,
            //            SpeedRelevant = false,
            //            Indoor = false,
            //            PointsPerMinute = 0.8
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = bikingActivity.Id,
            //            MeasurementType = "speed",
            //            DefaultDistanceUnit = "km",
            //            UseHeartRate = true,
            //            ElevationRelevant = true,
            //            DistanceRelevant = true,
            //            SpeedRelevant = true,
            //            Indoor = false,
            //            PointsPerMinute = 1.5
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = hikingActivity.Id,
            //            MeasurementType = "pace",
            //            DefaultDistanceUnit = "km",
            //            UseHeartRate = true,
            //            ElevationRelevant = true,
            //            DistanceRelevant = true,
            //            SpeedRelevant = true,
            //            Indoor = false,
            //            PointsPerMinute = 1.5
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = swimmingActivity.Id,
            //            MeasurementType = "pace",
            //            DefaultDistanceUnit = "km",
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = true,
            //            SpeedRelevant = true,
            //            Indoor = true,
            //            PointsPerMinute = 3
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = ellipticalActivity.Id,
            //            MeasurementType = "time",
            //            DefaultDistanceUnit = null,
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = false,
            //            SpeedRelevant = true,
            //            Indoor = true,
            //            PointsPerMinute = 2
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = jumpropeActivity.Id,
            //            MeasurementType = "reps",
            //            DefaultDistanceUnit = null,
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = false,
            //            SpeedRelevant = false,
            //            Indoor = true,
            //            PointsPerMinute = 1.6
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = weightTrainingActivity.Id,
            //            MeasurementType = "reps",
            //            DefaultDistanceUnit = null,
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = false,
            //            SpeedRelevant = false,
            //            Indoor = true,
            //            PointsPerMinute = 1
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = yogaActivity.Id,
            //            MeasurementType = "time",
            //            DefaultDistanceUnit = null,
            //            UseHeartRate = false,
            //            ElevationRelevant = false,
            //            DistanceRelevant = false,
            //            SpeedRelevant = false,
            //            Indoor = true,
            //            PointsPerMinute = 1
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = dancingActivity.Id,
            //            MeasurementType = "time",
            //            DefaultDistanceUnit = null,
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = false,
            //            SpeedRelevant = false,
            //            Indoor = true,
            //            PointsPerMinute = 2
            //        },
            //        new ActivityConfig
            //        {
            //            ActivityId = dancingActivity.Id,
            //            MeasurementType = "time",
            //            DefaultDistanceUnit = null,
            //            UseHeartRate = true,
            //            ElevationRelevant = false,
            //            DistanceRelevant = false,
            //            SpeedRelevant = false,
            //            Indoor = true,
            //            PointsPerMinute = 1
            //        }
            //    };

            //    context.ActivityConfig.AddRange(configs);
            //    await context.SaveChangesAsync();
            //}

            if (!context.SegmentConfigs.Any())
            {
                var configs = new List<SegmentConfig>
                {
                    // Run segments
                    new SegmentConfig { DistanceMeters = 400, Name = "400 Meters", ShortName = "400 m", ActivityId = runningActivity.Id },
                    new SegmentConfig { DistanceMeters = 1000, Name = "1 Kilometer", ShortName = "1 km", ActivityId = runningActivity.Id },
                    new SegmentConfig { DistanceMeters = 5000, Name = "5 Kilometers", ShortName = "5 km", ActivityId = runningActivity.Id },
                    new SegmentConfig { DistanceMeters = 10000, Name = "10 Kilometers", ShortName = "10 km", ActivityId = runningActivity.Id },
                    new SegmentConfig { DistanceMeters = 21097, Name = "Half Marathon", ShortName = "Half Marathon", ActivityId = runningActivity.Id },
                    new SegmentConfig { DistanceMeters = 42195, Name = "Marathon", ShortName = "Marathon", ActivityId = runningActivity.Id },

                    // Bike segments (typical cycling distances)
                    new SegmentConfig { DistanceMeters = 10000, Name = "10 Kilometers", ShortName = "10 km", ActivityId = bikingActivity.Id },
                    new SegmentConfig { DistanceMeters = 20000, Name = "20 Kilometers", ShortName = "20 km", ActivityId = bikingActivity.Id },
                    new SegmentConfig { DistanceMeters = 40000, Name = "40 Kilometers", ShortName = "40 km", ActivityId = bikingActivity.Id },
                    new SegmentConfig { DistanceMeters = 80000, Name = "80 Kilometers", ShortName = "80 km", ActivityId = bikingActivity.Id },
                    new SegmentConfig { DistanceMeters = 160000, Name = "160 Kilometers", ShortName = "160 km", ActivityId = bikingActivity.Id },

                    // Swim segments (typical swim distances in meters)
                    new SegmentConfig { DistanceMeters = 500, Name = "500 Meters", ShortName = "500 m", ActivityId = swimmingActivity.Id },
                    new SegmentConfig { DistanceMeters = 1000, Name = "1 Kilometer", ShortName = "1 km", ActivityId = swimmingActivity.Id },
                    new SegmentConfig { DistanceMeters = 1500, Name = "1500 Meters", ShortName = "1.5 km", ActivityId = swimmingActivity.Id },
                    new SegmentConfig { DistanceMeters = 3000, Name = "3 Kilometers", ShortName = "3 km", ActivityId = swimmingActivity.Id },
                };

                context.SegmentConfigs.AddRange(configs);
                await context.SaveChangesAsync();
            }

            if (!context.Medals.Any())
            {
                var medals = new List<Medal>
                {
                    // Running medals (Once - multiplier 3)
                    new Medal
                    {
                        Name = "1 km Run",
                        Description = "Complete 1 kilometer of running.",
                        TargetValue = 1000,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 5 * 1 * 1 * 3, // = 15
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "5 km Run",
                        Description = "Complete 5 kilometers of running.",
                        TargetValue = 5000,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 5 * 5 * 1 * 3, // = 75
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "10 km Run",
                        Description = "Complete 10 kilometers of running.",
                        TargetValue = 10000,
                        Level = 2,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 5 * 10 * 2 * 3, // = 300
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "25 km Run",
                        Description = "Complete 25 kilometers of running.",
                        TargetValue = 25000,
                        Level = 3,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 5 * 25 * 3 * 3, // = 1125
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "50 km Run",
                        Description = "Complete 50 kilometers of running.",
                        TargetValue = 50000,
                        Level = 4,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 5 * 50 * 4 * 3, // = 3000
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "100 km Run",
                        Description = "Complete 100 kilometers of running.",
                        TargetValue = 100000,
                        Level = 5,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 5 * 100 * 5 * 3, // = 7500
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },

                    // Weekly running medals (unchanged)
                    new Medal
                    {
                        Name = "Weekly 10 km Run",
                        Description = "Complete 10 kilometers of running in a week.",
                        TargetValue = 10000,
                        Level = 1,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 5 * 10 * 1 * 1, // = 50
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 25 km Run",
                        Description = "Complete 25 kilometers of running in a week.",
                        TargetValue = 25000,
                        Level = 2,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 5 * 25 * 2 * 1, // = 250
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 50 km Run",
                        Description = "Complete 50 kilometers of running in a week.",
                        TargetValue = 50000,
                        Level = 3,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 5 * 50 * 3 * 1, // = 750
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 100 km Run",
                        Description = "Complete 100 kilometers of running in a week.",
                        TargetValue = 100000,
                        Level = 4,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 5 * 100 * 4 * 1, // = 2000
                        CreatedAt = DateTime.Now,
                        ActivityId = runningActivity.Id
                    },

                    // Biking medals (Once - multiplier 3)
                    new Medal
                    {
                        Name = "25 km Bike Ride",
                        Description = "Complete 25 kilometers of biking.",
                        TargetValue = 25000,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 3 * 25 * 1 * 3, // = 225
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "50 km Bike Ride",
                        Description = "Complete 50 kilometers of biking.",
                        TargetValue = 50000,
                        Level = 2,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 3 * 50 * 2 * 3, // = 900
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "100 km Bike Ride",
                        Description = "Complete 100 kilometers of biking.",
                        TargetValue = 100000,
                        Level = 3,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 3 * 100 * 3 * 3, // = 2700
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "200 km Bike Ride",
                        Description = "Complete 200 kilometers of biking.",
                        TargetValue = 200000,
                        Level = 4,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 3 * 200 * 4 * 3, // = 7200
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },

                    // Weekly biking medals (unchanged)
                    new Medal
                    {
                        Name = "Weekly 50 km Bike Ride",
                        Description = "Complete 50 kilometers of biking in a week.",
                        TargetValue = 50000,
                        Level = 1,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 3 * 50 * 1 * 1, // = 150
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 100 km Bike Ride",
                        Description = "Complete 100 kilometers of biking in a week.",
                        TargetValue = 100000,
                        Level = 2,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 3 * 100 * 2 * 1, // = 600
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 200 km Bike Ride",
                        Description = "Complete 200 kilometers of biking in a week.",
                        TargetValue = 200000,
                        Level = 3,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 3 * 200 * 3 * 1, // = 1800
                        CreatedAt = DateTime.Now,
                        ActivityId = bikingActivity.Id
                    },

                    // Swimming medals (Once - multiplier 3)
                    new Medal
                    {
                        Name = "500 m Swim",
                        Description = "Complete 500 meters of swimming.",
                        TargetValue = 500,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = (int)(7 * 0.5 * 1 * 3), // = 10
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "1 km Swim",
                        Description = "Complete 1 kilometer of swimming.",
                        TargetValue = 1000,
                        Level = 2,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 7 * 1 * 2 * 3, // = 42
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "5 km Swim",
                        Description = "Complete 5 kilometers of swimming.",
                        TargetValue = 5000,
                        Level = 3,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 7 * 5 * 3 * 3, // = 315
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "10 km Swim",
                        Description = "Complete 10 kilometers of swimming.",
                        TargetValue = 10000,
                        Level = 4,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 7 * 10 * 4 * 3, // = 840
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "20 km Swim",
                        Description = "Complete 20 kilometers of swimming.",
                        TargetValue = 20000,
                        Level = 5,
                        Frequency = "Once",
                        IsOneTime = true,
                        Points = 7 * 20 * 5 * 3, // = 2100
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },

                    // Weekly swimming medals (unchanged)
                    new Medal
                    {
                        Name = "Weekly 1 km Swim",
                        Description = "Complete 1 kilometer of swimming in a week.",
                        TargetValue = 1000,
                        Level = 1,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 7 * 1 * 1 * 1, // = 7
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 2 km Swim",
                        Description = "Complete 2 kilometers of swimming in a week.",
                        TargetValue = 2000,
                        Level = 2,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 7 * 2 * 2 * 1, // = 28
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 5 km Swim",
                        Description = "Complete 5 kilometers of swimming in a week.",
                        TargetValue = 5000,
                        Level = 3,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 7 * 5 * 3 * 1, // = 105
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 10 km Swim",
                        Description = "Complete 10 kilometers of swimming in a week.",
                        TargetValue = 10000,
                        Level = 4,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        Points = 7 * 10 * 4 * 1, // = 280
                        CreatedAt = DateTime.Now,
                        ActivityId = swimmingActivity.Id
                    }
                };

                context.Medals.AddRange(medals);
                await context.SaveChangesAsync();
            }

            if (!context.Levels.Any())
            {
                // List of levels with the XP and total XP values
                var levels = new List<Level>
                    {
                        new Level { LevelNumber = 1, XP = 100, TotalXP = 100 },
                        new Level { LevelNumber = 2, XP = 300, TotalXP = 400 },
                        new Level { LevelNumber = 3, XP = 600, TotalXP = 1000 },
                        new Level { LevelNumber = 4, XP = 1000, TotalXP = 2000 },
                        new Level { LevelNumber = 5, XP = 1500, TotalXP = 3500 },
                        new Level { LevelNumber = 6, XP = 2100, TotalXP = 5600 },
                        new Level { LevelNumber = 7, XP = 2800, TotalXP = 8400 },
                        new Level { LevelNumber = 8, XP = 3600, TotalXP = 12000 },
                        new Level { LevelNumber = 9, XP = 4500, TotalXP = 17500 },
                        new Level { LevelNumber = 10, XP = 5500, TotalXP = 23000 },
                        new Level { LevelNumber = 11, XP = 6600, TotalXP = 29600 },
                        new Level { LevelNumber = 12, XP = 7800, TotalXP = 37400 },
                        new Level { LevelNumber = 13, XP = 9100, TotalXP = 46500 },
                        new Level { LevelNumber = 14, XP = 10500, TotalXP = 56000 },
                        new Level { LevelNumber = 15, XP = 12000, TotalXP = 68000 },
                        new Level { LevelNumber = 16, XP = 13600, TotalXP = 81600 },
                        new Level { LevelNumber = 17, XP = 15300, TotalXP = 96900 },
                        new Level { LevelNumber = 18, XP = 17100, TotalXP = 113400 },
                        new Level { LevelNumber = 19, XP = 19000, TotalXP = 132400 },
                        new Level { LevelNumber = 20, XP = 21000, TotalXP = 153900 },
                        new Level { LevelNumber = 21, XP = 23100, TotalXP = 177000 },
                        new Level { LevelNumber = 22, XP = 25300, TotalXP = 202500 },
                        new Level { LevelNumber = 23, XP = 27600, TotalXP = 230400 },
                        new Level { LevelNumber = 24, XP = 30000, TotalXP = 260700 },
                        new Level { LevelNumber = 25, XP = 32500, TotalXP = 293400 },
                        new Level { LevelNumber = 26, XP = 35100, TotalXP = 328500 },
                        new Level { LevelNumber = 27, XP = 37800, TotalXP = 366000 },
                        new Level { LevelNumber = 28, XP = 40600, TotalXP = 405900 },
                        new Level { LevelNumber = 29, XP = 43500, TotalXP = 448200 },
                        new Level { LevelNumber = 30, XP = 46500, TotalXP = 492900 },
                        new Level { LevelNumber = 31, XP = 49600, TotalXP = 540000 },
                        new Level { LevelNumber = 32, XP = 52800, TotalXP = 589500 },
                        new Level { LevelNumber = 33, XP = 56100, TotalXP = 641400 },
                        new Level { LevelNumber = 34, XP = 59500, TotalXP = 695700 },
                        new Level { LevelNumber = 35, XP = 63000, TotalXP = 752400 },
                        new Level { LevelNumber = 36, XP = 66600, TotalXP = 811500 },
                        new Level { LevelNumber = 37, XP = 70300, TotalXP = 873000 },
                        new Level { LevelNumber = 38, XP = 74100, TotalXP = 936900 },
                        new Level { LevelNumber = 39, XP = 78000, TotalXP = 1002000 },
                        new Level { LevelNumber = 40, XP = 82000, TotalXP = 1064000 },
                        new Level { LevelNumber = 41, XP = 86100, TotalXP = 1130100 },
                        new Level { LevelNumber = 42, XP = 90300, TotalXP = 1200300 },
                        new Level { LevelNumber = 43, XP = 94600, TotalXP = 1270300 },
                        new Level { LevelNumber = 44, XP = 99000, TotalXP = 1360300 },
                        new Level { LevelNumber = 45, XP = 103500, TotalXP = 1463800 },
                        new Level { LevelNumber = 46, XP = 108100, TotalXP = 1568900 },
                        new Level { LevelNumber = 47, XP = 112800, TotalXP = 1675700 },
                        new Level { LevelNumber = 48, XP = 117600, TotalXP = 1785300 },
                        new Level { LevelNumber = 49, XP = 122500, TotalXP = 1897800 },
                        new Level { LevelNumber = 50, XP = 127500, TotalXP = 2025300 },
                        new Level { LevelNumber = 51, XP = 132600, TotalXP = 2157900 },
                        new Level { LevelNumber = 52, XP = 137800, TotalXP = 2295700 },
                        new Level { LevelNumber = 53, XP = 143100, TotalXP = 2438800 },
                        new Level { LevelNumber = 54, XP = 148500, TotalXP = 2587300 },
                        new Level { LevelNumber = 55, XP = 154000, TotalXP = 2741300 },
                        new Level { LevelNumber = 56, XP = 159600, TotalXP = 2900900 },
                        new Level { LevelNumber = 57, XP = 165300, TotalXP = 3066200 },
                        new Level { LevelNumber = 58, XP = 171100, TotalXP = 3237300 },
                        new Level { LevelNumber = 59, XP = 177000, TotalXP = 3414300 },
                        new Level { LevelNumber = 60, XP = 183000, TotalXP = 3597300 },
                        new Level { LevelNumber = 61, XP = 189100, TotalXP = 3786400 },
                        new Level { LevelNumber = 62, XP = 195300, TotalXP = 3981700 },
                        new Level { LevelNumber = 63, XP = 201600, TotalXP = 4183300 },
                        new Level { LevelNumber = 64, XP = 208000, TotalXP = 4391300 },
                        new Level { LevelNumber = 65, XP = 214500, TotalXP = 4605800 },
                        new Level { LevelNumber = 66, XP = 221100, TotalXP = 4826800 },
                        new Level { LevelNumber = 67, XP = 227800, TotalXP = 5054300 },
                        new Level { LevelNumber = 68, XP = 234600, TotalXP = 5288400 },
                        new Level { LevelNumber = 69, XP = 241500, TotalXP = 5529100 },
                        new Level { LevelNumber = 70, XP = 248500, TotalXP = 5776500 },
                        new Level { LevelNumber = 71, XP = 255600, TotalXP = 6030600 },
                        new Level { LevelNumber = 72, XP = 262800, TotalXP = 6291500 },
                        new Level { LevelNumber = 73, XP = 270100, TotalXP = 6559600 },
                        new Level { LevelNumber = 74, XP = 277500, TotalXP = 6835100 },
                        new Level { LevelNumber = 75, XP = 285000, TotalXP = 7118100 },
                        new Level { LevelNumber = 76, XP = 292600, TotalXP = 7408700 },
                        new Level { LevelNumber = 77, XP = 300300, TotalXP = 7707000 },
                        new Level { LevelNumber = 78, XP = 308100, TotalXP = 8013100 },
                        new Level { LevelNumber = 79, XP = 316000, TotalXP = 8327100 },
                        new Level { LevelNumber = 80, XP = 324000, TotalXP = 8649100 },
                        new Level { LevelNumber = 81, XP = 332100, TotalXP = 8979100 },
                        new Level { LevelNumber = 82, XP = 340300, TotalXP = 9317100 },
                        new Level { LevelNumber = 83, XP = 348600, TotalXP = 9663200 },
                        new Level { LevelNumber = 84, XP = 357000, TotalXP = 10088000 },
                        new Level { LevelNumber = 85, XP = 365500, TotalXP = 10453500 },
                        new Level { LevelNumber = 86, XP = 374100, TotalXP = 10827600 },
                        new Level { LevelNumber = 87, XP = 382800, TotalXP = 11210400 },
                        new Level { LevelNumber = 88, XP = 391600, TotalXP = 11602000 },
                        new Level { LevelNumber = 89, XP = 400500, TotalXP = 12002500 },
                        new Level { LevelNumber = 90, XP = 409500, TotalXP = 12412000 },
                        new Level { LevelNumber = 91, XP = 418600, TotalXP = 12830500 },
                        new Level { LevelNumber = 92, XP = 427800, TotalXP = 13258300 },
                        new Level { LevelNumber = 93, XP = 437100, TotalXP = 13695500 },
                        new Level { LevelNumber = 94, XP = 446500, TotalXP = 14142400 },
                        new Level { LevelNumber = 95, XP = 456000, TotalXP = 14599100 },
                        new Level { LevelNumber = 96, XP = 465600, TotalXP = 15065600 },
                        new Level { LevelNumber = 97, XP = 475300, TotalXP = 15541900 },
                        new Level { LevelNumber = 98, XP = 485100, TotalXP = 16028000 },
                        new Level { LevelNumber = 99, XP = 495000, TotalXP = 16524000 },
                        new Level { LevelNumber = 100, XP = 505000, TotalXP = 17029000 }
                    };

                // Add the levels to the context
                context.Levels.AddRange(levels);

                // Save changes to the database
                await context.SaveChangesAsync();
            }
        }
    }
}