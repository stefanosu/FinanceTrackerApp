using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly FinanceTrackerDbContext _context;

        public TransactionService(FinanceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _context.Transactions
                .Where(t => !t.IsDeleted)
                .ToListAsync();
            return transactions.Select(MapToDto);
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.IsDeleted)
                throw new NotFoundException("Transaction", id);

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto)
        {
            if (dto == null)
                throw new ValidationException("Transaction cannot be null.");

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = dto.Type,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                Notes = dto.Notes ?? string.Empty
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> UpdateTransactionAsync(int id, UpdateTransactionDto dto)
        {
            if (dto == null)
                throw new ValidationException("Transaction cannot be null.");

            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null || existingTransaction.IsDeleted)
                throw new NotFoundException("Transaction", id);

            // Partial update: only update provided fields
            if (dto.AccountId.HasValue)
                existingTransaction.AccountId = dto.AccountId.Value;
            if (dto.Amount.HasValue)
                existingTransaction.Amount = dto.Amount.Value;
            if (!string.IsNullOrEmpty(dto.Type))
                existingTransaction.Type = dto.Type;
            if (dto.Date.HasValue)
                existingTransaction.Date = dto.Date.Value;
            if (dto.CategoryId.HasValue)
                existingTransaction.CategoryId = dto.CategoryId.Value;
            if (dto.Notes != null)
                existingTransaction.Notes = dto.Notes;

            await _context.SaveChangesAsync();
            return MapToDto(existingTransaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null || existingTransaction.IsDeleted)
                throw new NotFoundException("Transaction", id);

            // Soft delete: mark as deleted instead of removing
            existingTransaction.IsDeleted = true;
            existingTransaction.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Maps a Transaction entity to TransactionDto.
        /// </summary>
        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Date = transaction.Date,
                CategoryId = transaction.CategoryId,
                Notes = transaction.Notes
            };
        }
    }
}
