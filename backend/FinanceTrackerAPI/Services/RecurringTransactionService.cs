using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class RecurringTransactionService : IRecurringTransactionService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public RecurringTransactionService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<RecurringTransactionDto>> GetAllRecurringTransactionsAsync()
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var recurringTransactions = await _context.RecurringTransactions
                .Where(rt => rt.UserId == userId && !rt.IsDeleted)
                .ToListAsync();

            return recurringTransactions.Select(MapToDto);
        }

        public async Task<RecurringTransactionDto> GetRecurringTransactionByIdAsync(int id)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var recurringTransaction = await _context.RecurringTransactions
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (recurringTransaction == null || recurringTransaction.IsDeleted)
                throw new NotFoundException("RecurringTransaction", id);

            return MapToDto(recurringTransaction);
        }

        public async Task<RecurringTransactionDto> CreateRecurringTransactionAsync(CreateRecurringTransactionDto dto)
        {
            if (dto == null)
                throw new ValidationException("Recurring transaction cannot be null.");

            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var recurringTransaction = new RecurringTransaction
            {
                UserId = userId,
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Type = dto.Type,
                Frequency = dto.Frequency,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                NextDueDate = dto.StartDate,
                IsActive = true
            };

            await _context.RecurringTransactions.AddAsync(recurringTransaction);
            await _context.SaveChangesAsync();

            return MapToDto(recurringTransaction);
        }

        public async Task<RecurringTransactionDto> UpdateRecurringTransactionAsync(int id, UpdateRecurringTransactionDto dto)
        {
            if (dto == null)
                throw new ValidationException("Recurring transaction cannot be null.");

            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var existingTransaction = await _context.RecurringTransactions
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (existingTransaction == null || existingTransaction.IsDeleted)
                throw new NotFoundException("RecurringTransaction", id);

            // Partial update: only update provided fields
            if (dto.AccountId.HasValue)
                existingTransaction.AccountId = dto.AccountId.Value;
            if (dto.Amount.HasValue)
                existingTransaction.Amount = dto.Amount.Value;
            if (!string.IsNullOrEmpty(dto.Type))
                existingTransaction.Type = dto.Type;
            if (!string.IsNullOrEmpty(dto.Frequency))
                existingTransaction.Frequency = dto.Frequency;
            if (!string.IsNullOrEmpty(dto.Description))
                existingTransaction.Description = dto.Description;
            if (dto.CategoryId.HasValue)
                existingTransaction.CategoryId = dto.CategoryId.Value;
            if (dto.StartDate.HasValue)
                existingTransaction.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue)
                existingTransaction.EndDate = dto.EndDate.Value;
            if (dto.IsActive.HasValue)
                existingTransaction.IsActive = dto.IsActive.Value;

            await _context.SaveChangesAsync();
            return MapToDto(existingTransaction);
        }

        public async Task<bool> DeleteRecurringTransactionAsync(int id)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var existingTransaction = await _context.RecurringTransactions
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.UserId == userId);

            if (existingTransaction == null || existingTransaction.IsDeleted)
                throw new NotFoundException("RecurringTransaction", id);

            // Soft delete
            existingTransaction.IsDeleted = true;
            existingTransaction.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TransactionDto>> ProcessDueTransactionsAsync()
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createdTransactions = new List<Transaction>();

            var dueTransactions = await _context.RecurringTransactions
                .Where(rt => rt.UserId == userId
                    && !rt.IsDeleted
                    && rt.IsActive
                    && rt.NextDueDate <= today
                    && (!rt.EndDate.HasValue || rt.EndDate.Value >= today))
                .ToListAsync();

            foreach (var recurring in dueTransactions)
            {
                // Create the actual transaction
                var transaction = new Transaction
                {
                    AccountId = recurring.AccountId,
                    Amount = recurring.Amount,
                    Type = recurring.Type,
                    Date = recurring.NextDueDate,
                    CategoryId = recurring.CategoryId,
                    Notes = $"[Auto] {recurring.Description}"
                };

                await _context.Transactions.AddAsync(transaction);
                createdTransactions.Add(transaction);

                // Calculate next due date
                recurring.NextDueDate = CalculateNextDueDate(recurring.NextDueDate, recurring.Frequency);

                // Deactivate if past end date
                if (recurring.EndDate.HasValue && recurring.NextDueDate > recurring.EndDate.Value)
                {
                    recurring.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();

            return createdTransactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Type = t.Type,
                Date = t.Date,
                CategoryId = t.CategoryId,
                Notes = t.Notes
            });
        }

        private static DateOnly CalculateNextDueDate(DateOnly currentDate, string frequency)
        {
            return frequency.ToLower() switch
            {
                "daily" => currentDate.AddDays(1),
                "weekly" => currentDate.AddDays(7),
                "biweekly" => currentDate.AddDays(14),
                "monthly" => currentDate.AddMonths(1),
                "quarterly" => currentDate.AddMonths(3),
                "yearly" => currentDate.AddYears(1),
                _ => currentDate.AddMonths(1) // Default to monthly
            };
        }

        private static RecurringTransactionDto MapToDto(RecurringTransaction recurringTransaction)
        {
            return new RecurringTransactionDto
            {
                Id = recurringTransaction.Id,
                AccountId = recurringTransaction.AccountId,
                Amount = recurringTransaction.Amount,
                Type = recurringTransaction.Type,
                Frequency = recurringTransaction.Frequency,
                Description = recurringTransaction.Description,
                CategoryId = recurringTransaction.CategoryId,
                StartDate = recurringTransaction.StartDate,
                EndDate = recurringTransaction.EndDate,
                NextDueDate = recurringTransaction.NextDueDate,
                IsActive = recurringTransaction.IsActive
            };
        }
    }
}
