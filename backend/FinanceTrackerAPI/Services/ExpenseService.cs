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

            // Get user's household if any
            var userHouseholdId = await GetUserHouseholdIdAsync(userId.Value);

            // Get user's own expenses + shared household expenses
            var expenses = await _context.Expenses
                .Include(e => e.Household)
                .Where(e => e.UserId == userId.Value ||
                           (userHouseholdId != null && e.HouseholdId == userHouseholdId))
                .ToListAsync();

            // Get user info for owner names
            var userIds = expenses.Select(e => e.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}");

            return expenses.Select(e => MapToDto(e, users.GetValueOrDefault(e.UserId, "Unknown")));
        }

        private async Task<int?> GetUserHouseholdIdAsync(int userId)
        {
            var membership = await _context.HouseholdMembers
                .FirstOrDefaultAsync(hm => hm.UserId == userId);
            return membership?.HouseholdId;
        }

        public async Task<ExpenseDto> GetExpenseByIdAsync(int id)
        {
            var userId = _currentUserService.GetUserId();
            var userHouseholdId = userId != null ? await GetUserHouseholdIdAsync(userId.Value) : null;

            var expense = await _context.Expenses
                .Include(e => e.Household)
                .FirstOrDefaultAsync(e => e.Id == id);

            // Allow access if user owns expense OR expense is shared with user's household
            var canAccess = expense != null && (
                expense.UserId == userId ||
                (userHouseholdId != null && expense.HouseholdId == userHouseholdId)
            );

            if (expense == null || !canAccess)
                throw new NotFoundException("Expense", id);

            var owner = await _context.Users.FindAsync(expense.UserId);
            return MapToDto(expense, owner != null ? $"{owner.FirstName} {owner.LastName}" : "Unknown");
        }

        public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto)
        {
            if (dto == null)
                throw new ValidationException("Expense cannot be null.");

            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            // Get household ID if sharing
            int? householdId = null;
            if (dto.IsShared)
            {
                householdId = await GetUserHouseholdIdAsync(userId.Value);
                if (householdId == null)
                    throw new ValidationException("You must be a member of a household to share expenses");
            }

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
                UserId = userId.Value,
                Mood = ParseMood(dto.Mood),
                HouseholdId = householdId
            };

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId.Value);
            return MapToDto(expense, user != null ? $"{user.FirstName} {user.LastName}" : "Unknown");
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
            if (dto.Mood != null)
                existingExpense.Mood = ParseMood(dto.Mood);
            if (dto.IsShared.HasValue)
            {
                if (dto.IsShared.Value)
                {
                    var householdId = await GetUserHouseholdIdAsync(userId!.Value);
                    if (householdId == null)
                        throw new ValidationException("You must be a member of a household to share expenses");
                    existingExpense.HouseholdId = householdId;
                }
                else
                {
                    existingExpense.HouseholdId = null;
                }
            }

            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(existingExpense.UserId);
            return MapToDto(existingExpense, user != null ? $"{user.FirstName} {user.LastName}" : "Unknown");
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
        private static ExpenseDto MapToDto(Expense expense, string ownerName = "")
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
                Notes = expense.Notes,
                Mood = expense.Mood?.ToString(),
                HouseholdId = expense.HouseholdId,
                IsShared = expense.HouseholdId != null,
                OwnerName = ownerName
            };
        }

        private static Mood? ParseMood(string? moodString)
        {
            if (string.IsNullOrEmpty(moodString))
                return null;

            if (Enum.TryParse<Mood>(moodString, ignoreCase: true, out var mood))
                return mood;

            return null;
        }
    }
}
