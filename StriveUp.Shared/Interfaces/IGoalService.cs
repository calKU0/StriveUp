using StriveUp.Shared.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IGoalService
    {
        Task<List<UserGoalDto>> GetGoalsAsync();

        Task<bool> AddGoalAsync(CreateUserGoalDto goal);

        Task<bool> DeleteGoalAsync(int goalId);
    }
}