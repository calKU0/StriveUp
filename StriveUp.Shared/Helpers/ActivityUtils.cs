using StriveUp.Shared.DTOs;
using StriveUp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
