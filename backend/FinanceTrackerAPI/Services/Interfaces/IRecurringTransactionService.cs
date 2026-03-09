using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IRecurringTransactionService
    {
        Task<IEnumerable<RecurringTransactionDto>> GetAllRecurringTransactionsAsync();
        Task<RecurringTransactionDto> GetRecurringTransactionByIdAsync(int id);
        Task<RecurringTransactionDto> CreateRecurringTransactionAsync(CreateRecurringTransactionDto dto);
        Task<RecurringTransactionDto> UpdateRecurringTransactionAsync(int id, UpdateRecurringTransactionDto dto);
        Task<bool> DeleteRecurringTransactionAsync(int id);
        Task<IEnumerable<TransactionDto>> ProcessDueTransactionsAsync();
    }
}
