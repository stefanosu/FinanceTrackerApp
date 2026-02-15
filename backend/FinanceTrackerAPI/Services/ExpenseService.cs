using backend.Services.Interfaces;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
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

        public async Task<IEnumerable<ExpenseDto>> GetAllExpensesAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return Enumerable.Empty<ExpenseDto>();

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value)
                .ToListAsync();

            return expenses.Select(MapToDto);
        }

        public async Task<ExpenseDto> GetExpenseByIdAsync(int id)
        {
            var userId = _currentUserService.GetUserId();
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null || (userId != null && expense.UserId != userId.Value))
                throw new NotFoundException("Expense", id);

            return MapToDto(expense);
        }

        public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto)
        {
            if (dto == null)
                throw new ValidationException("Expense cannot be null.");

            var userId = _currentUserService.GetUserId();

            // Map DTO to entity
            var expense = new Expense
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                Amount = dto.Amount,
                Date = dto.Date,
                Category = dto.Category,
                SubCategory = dto.SubCategory ?? string.Empty,
                PaymentMethod = dto.PaymentMethod ?? string.Empty,
                Notes = dto.Notes ?? string.Empty,
                UserId = userId ?? 0
            };

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            return MapToDto(expense);
        }

        public async Task<ExpenseDto> UpdateExpenseAsync(int id, UpdateExpenseDto dto)
        {
            if (dto == null)
                throw new ValidationException("Expense cannot be null.");

            var userId = _currentUserService.GetUserId();
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null || (userId != null && existingExpense.UserId != userId.Value))
                throw new NotFoundException("Expense", id);

            // Update only provided fields (partial update)
            if (!string.IsNullOrEmpty(dto.Name))
                existingExpense.Name = dto.Name;
            if (dto.Description != null)
                existingExpense.Description = dto.Description;
            if (dto.Amount.HasValue)
                existingExpense.Amount = dto.Amount.Value;
            if (dto.Date.HasValue)
                existingExpense.Date = dto.Date.Value;
            if (!string.IsNullOrEmpty(dto.Category))
                existingExpense.Category = dto.Category;
            if (dto.SubCategory != null)
                existingExpense.SubCategory = dto.SubCategory;
            if (dto.PaymentMethod != null)
                existingExpense.PaymentMethod = dto.PaymentMethod;
            if (dto.Notes != null)
                existingExpense.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return MapToDto(existingExpense);
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

        /// <summary>
        /// Maps an Expense entity to ExpenseDto.
        /// Centralizes mapping logic and ensures sensitive fields are excluded.
        /// </summary>
        private static ExpenseDto MapToDto(Expense expense)
        {
            return new ExpenseDto
            {
                Id = expense.Id,
                Name = expense.Name,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category,
                SubCategory = expense.SubCategory,
                PaymentMethod = expense.PaymentMethod,
                Notes = expense.Notes
            };
        }
    }
}
