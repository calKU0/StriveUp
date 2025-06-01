using StriveUp.Shared.DTOs;
using StriveUp.Shared.DTOs.Profile;

namespace StriveUp.Shared.Interfaces
{
    public interface IFollowService
    {
        Task<List<UserFollowDto>> SearchUsersAsync(string keyword);

        Task<bool> FollowAsync(string followedId);

        Task<bool> UnfollowAsync(string followedId);

        Task<List<UserFollowDto>> GetUserFollowers(string userName);

        Task<List<UserFollowDto>> GetUserFollowing(string userName);
    }
}