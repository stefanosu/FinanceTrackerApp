using Microsoft.EntityFrameworkCore;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly FinanceTrackerDbContext _context;

        public ExpenseService(FinanceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                throw new NotFoundException("Expense", id);

            return expense;
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            if (expense == null)
                throw new ValidationException("Expense cannot be null.");

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(int id, Expense expense)
        {
            if (expense == null)
                throw new ValidationException("Expense cannot be null.");

            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
                throw new NotFoundException("Expense", id);

            existingExpense.Name = expense.Name;
            existingExpense.Description = expense.Description;
            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Category = expense.Category;
            existingExpense.SubCategory = expense.SubCategory;
            existingExpense.PaymentMethod = expense.PaymentMethod;
            existingExpense.Notes = expense.Notes;

            await _context.SaveChangesAsync();
            return existingExpense;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
                throw new NotFoundException("Expense", id);

            _context.Expenses.Remove(existingExpense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
