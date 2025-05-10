using StriveUp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StriveUp.Infrastructure.Data
{
    public static class Seed
    {
        public static void SeedData(AppDbContext context)
        {
            if (!context.Activities.Any())
            {
                var activities = new List<Activity>
                {
                    new Activity
                    {
                        Name = "Run",
                        Description = "A basic running activity.",
                        AverageCaloriesPerHour = 500
                    },
                    new Activity
                    {
                        Name = "Bike",
                        Description = "A biking activity.",
                        AverageCaloriesPerHour = 600
                    },
                    new Activity
                    {
                        Name = "Swim",
                        Description = "A swimming activity.",
                        AverageCaloriesPerHour = 400
                    }
                };

                context.Activities.AddRange(activities);
                context.SaveChanges();
            }

            var runningActivity = context.Activities.First(a => a.Name == "Run");
            var bikingActivity = context.Activities.First(a => a.Name == "Bike");
            var swimmingActivity = context.Activities.First(a => a.Name == "Swim");

            if (!context.ActivityConfig.Any())
            {
                var configs = new List<ActivityConfig>
                {
                    new ActivityConfig
                    {
                        Id = 1,
                        ActivityId = runningActivity.Id,
                        MeasurementType = "pace",
                        DefaultDistanceUnit = "km",
                        UseHeartRate = true,
                        ElevationRelevant = false,
                        IndoorCapable = true,
                    },
                    new ActivityConfig
                    {
                        Id = 2,
                        ActivityId = bikingActivity.Id,
                        MeasurementType = "speed",
                        DefaultDistanceUnit = "km",
                        UseHeartRate = true,
                        ElevationRelevant = true,
                        IndoorCapable = false,
                    },
                    new ActivityConfig
                    {
                        Id = 3,
                        ActivityId = swimmingActivity.Id,
                        MeasurementType = "pace",
                        DefaultDistanceUnit = "m",
                        UseHeartRate = false,
                        ElevationRelevant = false,
                        IndoorCapable = true,
                    }
                };
            }

            if (!context.Medals.Any())
            {
                var medals = new List<Medal>
                {
                    // Running medals
                    new Medal
                    {
                        Name = "1 km Run",
                        Description = "Complete 1 kilometer of running.",
                        TargetValue = 1,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/1km_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "5 km Run",
                        Description = "Complete 5 kilometers of running.",
                        TargetValue = 5,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/5km_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "10 km Run",
                        Description = "Complete 10 kilometers of running.",
                        TargetValue = 10,
                        Level = 2,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/10km_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "25 km Run",
                        Description = "Complete 25 kilometers of running.",
                        TargetValue = 25,
                        Level = 3,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/25km_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "50 km Run",
                        Description = "Complete 50 kilometers of running.",
                        TargetValue = 50,
                        Level = 4,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/50km_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "100 km Run",
                        Description = "Complete 100 kilometers of running.",
                        TargetValue = 100,
                        Level = 5,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/100km_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 10 km Run",
                        Description = "Complete 10 kilometers of running in a week.",
                        TargetValue = 10,
                        Level = 1,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/10km_weekly_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 25 km Run",
                        Description = "Complete 25 kilometers of running in a week.",
                        TargetValue = 25,
                        Level = 2,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/25km_weekly_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 50 km Run",
                        Description = "Complete 50 kilometers of running in a week.",
                        TargetValue = 50,
                        Level = 3,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/50km_weekly_run.png",
                        ActivityId = runningActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 100 km Run",
                        Description = "Complete 100 kilometers of running in a week.",
                        TargetValue = 100,
                        Level = 4,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/100km_weekly_run.png",
                        ActivityId = runningActivity.Id
                    },

                    // Biking medals
                    new Medal
                    {
                        Name = "25 km Bike Ride",
                        Description = "Complete 25 kilometers of biking.",
                        TargetValue = 25,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/25km_bike.png",
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "50 km Bike Ride",
                        Description = "Complete 50 kilometers of biking.",
                        TargetValue = 50,
                        Level = 2,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/50km_bike.png",
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "100 km Bike Ride",
                        Description = "Complete 100 kilometers of biking.",
                        TargetValue = 100,
                        Level = 3,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/100km_bike.png",
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "200 km Bike Ride",
                        Description = "Complete 200 kilometers of biking.",
                        TargetValue = 200,
                        Level = 4,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/200km_bike.png",
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 50 km Bike Ride",
                        Description = "Complete 50 kilometers of biking in a week.",
                        TargetValue = 50,
                        Level = 1,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/50km_weekly_bike.png",
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 100 km Bike Ride",
                        Description = "Complete 100 kilometers of biking in a week.",
                        TargetValue = 100,
                        Level = 2,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/100km_weekly_bike.png",
                        ActivityId = bikingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 200 km Bike Ride",
                        Description = "Complete 200 kilometers of biking in a week.",
                        TargetValue = 200,
                        Level = 3,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/200km_weekly_bike.png",
                        ActivityId = bikingActivity.Id
                    },

                    // Swimming medals
                    new Medal
                    {
                        Name = "500 m Swim",
                        Description = "Complete 500 meters of swimming.",
                        TargetValue = 0.5,
                        Level = 1,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/500m_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "1 km Swim",
                        Description = "Complete 1 kilometer of swimming.",
                        TargetValue = 1,
                        Level = 2,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/1km_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "5 km Swim",
                        Description = "Complete 5 kilometers of swimming.",
                        TargetValue = 5,
                        Level = 3,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/5km_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "10 km Swim",
                        Description = "Complete 10 kilometers of swimming.",
                        TargetValue = 10,
                        Level = 4,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/10km_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "20 km Swim",
                        Description = "Complete 20 kilometers of swimming.",
                        TargetValue = 20,
                        Level = 5,
                        Frequency = "Once",
                        IsOneTime = true,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/20km_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 1 km Swim",
                        Description = "Complete 1 kilometer of swimming in a week.",
                        TargetValue = 1,
                        Level = 1,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/1km_weekly_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 2 km Swim",
                        Description = "Complete 2 kilometers of swimming in a week.",
                        TargetValue = 2,
                        Level = 2,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/2km_weekly_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 5 km Swim",
                        Description = "Complete 5 kilometers of swimming in a week.",
                        TargetValue = 5,
                        Level = 3,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/5km_weekly_swim.png",
                        ActivityId = swimmingActivity.Id
                    },
                    new Medal
                    {
                        Name = "Weekly 10 km Swim",
                        Description = "Complete 10 kilometers of swimming in a week.",
                        TargetValue = 10,
                        Level = 4,
                        Frequency = "Weekly",
                        IsOneTime = false,
                        CreatedAt = DateTime.Now,
                        ImageUrl = "https://example.com/medals/10km_weekly_swim.png",
                        ActivityId = swimmingActivity.Id
                    }
                };

                context.Medals.AddRange(medals);
                context.SaveChanges();
            }
        }
    }
}
