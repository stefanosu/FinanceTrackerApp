using FinanceTrackerAPI.Services.Dtos;

namespace backend.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDto>> GetAllExpensesAsync();
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto);
        Task<ExpenseDto> UpdateExpenseAsync(int id, UpdateExpenseDto dto);
        Task<bool> DeleteExpenseAsync(int id);
    }
}
