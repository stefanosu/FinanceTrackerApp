using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<ExpenseCategory>> GetAllCategoriesAsync();
        Task<ExpenseCategory> GetCategoryByIdAsync(int id);
        Task<ExpenseCategory> CreateCategoryAsync(ExpenseCategory category);
        Task<ExpenseCategory> UpdateCategoryAsync(int id, ExpenseCategory category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
