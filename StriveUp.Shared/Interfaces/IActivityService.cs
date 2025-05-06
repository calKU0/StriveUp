using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IActivityService
    {
        Task<List<UserActivityDto>?> GetUserActivitiesAsync();
        Task<bool> AddActivityAsync(UserActivityDto activity);
        Task<bool> UpdateActivityAsync(int activityId, UserActivityDto activity);
        Task<List<ActivityDto>?> GetAvailableActivitiesAsync();
        Task<UserActivityDto?> GetActivityByIdAsync(int activityId);
        Task AddCommentAsync(int activityId, string comment);
        Task LikeActivityAsync(int activityId);
    }
}
