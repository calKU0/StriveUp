﻿using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;

namespace StriveUp.Shared.Helpers
{
    public static class ActivityUtils
    {
        public static string GetActivityImageUrl(string activityName, string theme)
        {
            return activityName switch
            {
                "Run" => $"images/icons/run-{theme}.webp",
                "Bike" => $"images/icons/bike-{theme}.webp",
                "Swim" => $"images/icons/swim-{theme}.webp",
                _ => $"images/icons/default.png"
            };
        }

        public static async Task ToggleLikeAsync(
            UserActivityDto activity,
            string currentUserId,
            IActivityService activityService)
        {
            try
            {
                // Optimistically update UI
                activity.LikeCount += activity.IsLikedByCurrentUser ? -1 : 1;
                activity.IsLikedByCurrentUser = !activity.IsLikedByCurrentUser;

                await activityService.LikeActivityAsync(activity.Id);
            }
            catch (Exception ex)
            {
                activity.LikeCount += activity.IsLikedByCurrentUser ? -1 : 1;
                activity.IsLikedByCurrentUser = !activity.IsLikedByCurrentUser;
            }
        }

        public static string GetSpeedOrPace(double value, string measurementType)
        {
            if (measurementType == "pace")
            {
                // Convert m/s to pace (min/km) without rounding
                double? paceInMinPerKm = 1000.0 / value / 60.0;

                int minutes = (int)paceInMinPerKm;
                int seconds = (int)((paceInMinPerKm - minutes) * 60);

                if (seconds == 60)
                {
                    minutes++;
                    seconds = 0;
                }

                return $"{minutes}:{seconds:D2}";
            }
            else
            {
                double? speedInKmH = value * 3.6;
                return $"{speedInKmH:F2}";
            }
        }

        public static string GetSpeedOrPaceLabel(string measurementType)
        {
            return measurementType == "pace" ? "Pace" : "Speed";
        }
    }
}