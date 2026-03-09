using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface ISavingsGoalService
    {
        Task<IEnumerable<SavingsGoalDto>> GetAllSavingsGoalsAsync();
        Task<SavingsGoalDto> GetSavingsGoalByIdAsync(int id);
        Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalDto dto);
        Task<SavingsGoalDto> UpdateSavingsGoalAsync(int id, UpdateSavingsGoalDto dto);
        Task<bool> DeleteSavingsGoalAsync(int id);
        Task<SavingsGoalDto> ContributeToGoalAsync(int id, ContributeToGoalDto dto);
    }
}
