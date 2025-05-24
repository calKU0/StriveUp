using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface IFollowService
    {
        Task<List<FollowDto>> SearchUsersAsync(string keyword);

        Task FollowAsync(string followerId, string followedId);

        Task UnfollowAsync(string followerId, string followedId);
    }
}