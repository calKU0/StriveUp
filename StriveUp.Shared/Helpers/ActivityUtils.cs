using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StriveUp.Shared.DTOs.Activity;

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
            IActivityService activityService,
            INotificationService notificationService)
        {
            await activityService.LikeActivityAsync(activity.Id);

            if (currentUserId != activity.UserId && !activity.IsLikedByCurrentUser)
            {
                var notificationDto = new CreateNotificationDto
                {
                    UserId = activity.UserId,
                    Title = "New Like",
                    ActorId = currentUserId,
                    Type = "like",
                    Message = "liked your activity",
                    RedirectUrl = $"/activity/{activity.Id}",
                };
                await notificationService.CreateNotificationAsync(notificationDto);
            }

            activity.LikeCount += activity.IsLikedByCurrentUser ? -1 : 1;
            activity.IsLikedByCurrentUser = !activity.IsLikedByCurrentUser;
        }

        public static string GetSpeedOrPace(double? value, ActivityDto? activityConfig)
        {
            if (activityConfig != null)
            {
                if (activityConfig.MeasurementType == "pace")
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

                    return $"{minutes}:{seconds:D2} min/km";
                }
                else
                {
                    double? speedInKmH = value * 3.6;
                    return $"{speedInKmH:F2} km/h";
                }
            }

            return string.Empty;
        }

        public static string GetSpeedOrPaceLabel(ActivityDto? activityConfig)
        {
            if (activityConfig != null)
            {
                return activityConfig.MeasurementType == "pace" ? "Pace" : "Speed";
            }

            return string.Empty;
        }
    }

}
