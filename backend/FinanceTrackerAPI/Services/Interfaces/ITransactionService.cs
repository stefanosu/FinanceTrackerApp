using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> UpdateTransactionAsync(int id, Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
    }
}
