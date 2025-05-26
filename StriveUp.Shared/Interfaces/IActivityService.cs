using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Activity;

namespace StriveUp.Shared.Interfaces
{
    public interface IActivityService
    {
        Task<List<UserActivityDto>> GetFeedAsync(int page, int pageSize);

        Task<List<UserActivityDto>> GetUserActivitiesAsync(string userName, int page, int pageSize);

        Task<bool> AddActivityAsync(CreateUserActivityDto activity);

        Task<bool> UpdateUserActivityAsync(int activityId, UpdateUserActivityDto activity);
        Task<bool> DeleteUserActivityAsync(int activityId);

        Task<List<ActivityDto>?> GetAvailableActivitiesAsync();

        Task<List<ActivityDto>?> GetActivitiesWithSegments();

        Task<UserActivityDto?> GetActivityByIdAsync(int activityId);

        Task<ActivityDto> GetActivityConfig(int id);

        Task<List<ActivityCommentDto>?> GetActivityComments(int activityId);

        Task AddCommentAsync(int activityId, string comment);

        Task LikeActivityAsync(int activityId);
    }
}