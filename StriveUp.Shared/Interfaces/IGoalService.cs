using StriveUp.Shared.DTOs.Profile;

namespace StriveUp.Shared.Interfaces
{
    public interface IGoalService
    {
        Task<List<UserGoalDto>> GetGoalsAsync();

        Task<bool> AddGoalAsync(CreateUserGoalDto goal);

        Task<bool> DeleteGoalAsync(int goalId);
    }
}