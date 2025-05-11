using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IFollowService
    {
        Task<List<FollowDto>> SearchUsersAsync(string keyword);
        Task FollowAsync(string followerId, string followedId);
        Task UnfollowAsync(string followerId, string followedId);
    }
}
