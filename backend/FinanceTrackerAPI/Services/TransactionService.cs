using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
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

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                throw new NotFoundException("Transaction", id);

            return transaction;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ValidationException("Transaction cannot be null.");

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(int id, Transaction transaction)
        {
            if (transaction == null)
                throw new ValidationException("Transaction cannot be null.");

            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
                throw new NotFoundException("Transaction", id);

            // Update transaction properties here
            // Note: This is a placeholder - you'll need to define which properties to update
            // existingTransaction.Property = transaction.Property;

            await _context.SaveChangesAsync();
            return existingTransaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
                throw new NotFoundException("Transaction", id);

            _context.Transactions.Remove(existingTransaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}