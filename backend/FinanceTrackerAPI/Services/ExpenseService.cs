using backend.Services.Interfaces;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ExpenseService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return Enumerable.Empty<Expense>();

            return await _context.Expenses
                .Where(e => e.UserId == userId.Value)
                .ToListAsync();
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            var userId = _currentUserService.GetUserId();
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null || (userId != null && expense.UserId != userId.Value))
                throw new NotFoundException("Expense", id);

            return expense;
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            if (expense == null)
                throw new ValidationException("Expense cannot be null.");

            var userId = _currentUserService.GetUserId();
            if (userId != null)
                expense.UserId = userId.Value;

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(int id, Expense expense)
        {
            if (expense == null)
                throw new ValidationException("Expense cannot be null.");

            var userId = _currentUserService.GetUserId();
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null || (userId != null && existingExpense.UserId != userId.Value))
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
            var userId = _currentUserService.GetUserId();
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null || (userId != null && existingExpense.UserId != userId.Value))
                throw new NotFoundException("Expense", id);

            // Soft delete: mark as deleted instead of removing
            existingExpense.IsDeleted = true;
            existingExpense.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
