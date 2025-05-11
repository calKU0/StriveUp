using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IActivityService
    {
        Task<List<UserActivityDto>?> GetFeedAsync();
        Task<bool> AddActivityAsync(CreateUserActivityDto activity);
        Task<bool> UpdateActivityAsync(int activityId, CreateUserActivityDto activity);
        Task<List<ActivityDto>?> GetAvailableActivitiesAsync();
        Task<UserActivityDto?> GetActivityByIdAsync(int activityId);
        Task<List<ActivityCommentDto>?> GetActivityComments(int activityId);
        Task AddCommentAsync(int activityId, string comment);
        Task LikeActivityAsync(int activityId);
    }
}
