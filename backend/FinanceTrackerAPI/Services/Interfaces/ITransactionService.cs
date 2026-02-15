using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto> GetTransactionByIdAsync(int id);
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto);
        Task<TransactionDto> UpdateTransactionAsync(int id, UpdateTransactionDto dto);
        Task<bool> DeleteTransactionAsync(int id);
    }
}
