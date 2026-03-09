using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class SavingsGoalService : ISavingsGoalService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SavingsGoalService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<SavingsGoalDto>> GetAllSavingsGoalsAsync()
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var goals = await _context.SavingsGoals
                .Where(g => g.UserId == userId && !g.IsDeleted)
                .OrderByDescending(g => g.Id)
                .ToListAsync();

            return goals.Select(MapToDto);
        }

        public async Task<SavingsGoalDto> GetSavingsGoalByIdAsync(int id)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null || goal.IsDeleted)
                throw new NotFoundException("SavingsGoal", id);

            return MapToDto(goal);
        }

        public async Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalDto dto)
        {
            if (dto == null)
                throw new ValidationException("Savings goal cannot be null.");

            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var goal = new SavingsGoal
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                TargetAmount = dto.TargetAmount,
                CurrentAmount = dto.InitialAmount ?? 0,
                TargetDate = dto.TargetDate,
                AccountId = dto.AccountId,
                Category = dto.Category
            };

            await _context.SavingsGoals.AddAsync(goal);
            await _context.SaveChangesAsync();

            return MapToDto(goal);
        }

        public async Task<SavingsGoalDto> UpdateSavingsGoalAsync(int id, UpdateSavingsGoalDto dto)
        {
            if (dto == null)
                throw new ValidationException("Savings goal cannot be null.");

            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var existingGoal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (existingGoal == null || existingGoal.IsDeleted)
                throw new NotFoundException("SavingsGoal", id);

            // Partial update
            if (!string.IsNullOrEmpty(dto.Name))
                existingGoal.Name = dto.Name;
            if (dto.Description != null)
                existingGoal.Description = dto.Description;
            if (dto.TargetAmount.HasValue)
                existingGoal.TargetAmount = dto.TargetAmount.Value;
            if (dto.TargetDate.HasValue)
                existingGoal.TargetDate = dto.TargetDate.Value;
            if (dto.AccountId.HasValue)
                existingGoal.AccountId = dto.AccountId.Value;
            if (!string.IsNullOrEmpty(dto.Category))
                existingGoal.Category = dto.Category;

            // Check if goal is now completed
            CheckAndMarkCompletion(existingGoal);

            await _context.SaveChangesAsync();
            return MapToDto(existingGoal);
        }

        public async Task<bool> DeleteSavingsGoalAsync(int id)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var existingGoal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (existingGoal == null || existingGoal.IsDeleted)
                throw new NotFoundException("SavingsGoal", id);

            // Soft delete
            existingGoal.IsDeleted = true;
            existingGoal.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SavingsGoalDto> ContributeToGoalAsync(int id, ContributeToGoalDto dto)
        {
            if (dto == null)
                throw new ValidationException("Contribution cannot be null.");

            if (dto.Amount <= 0)
                throw new ValidationException("Contribution amount must be greater than zero.");

            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal == null || goal.IsDeleted)
                throw new NotFoundException("SavingsGoal", id);

            if (goal.IsCompleted)
                throw new ValidationException("Cannot contribute to a completed goal.");

            goal.CurrentAmount += dto.Amount;

            // Check if goal is now completed
            CheckAndMarkCompletion(goal);

            await _context.SaveChangesAsync();
            return MapToDto(goal);
        }

        private static void CheckAndMarkCompletion(SavingsGoal goal)
        {
            if (!goal.IsCompleted && goal.CurrentAmount >= goal.TargetAmount)
            {
                goal.IsCompleted = true;
                goal.CompletedAt = DateTime.UtcNow;
            }
        }

        private static SavingsGoalDto MapToDto(SavingsGoal goal)
        {
            return new SavingsGoalDto
            {
                Id = goal.Id,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                ProgressPercentage = goal.ProgressPercentage,
                TargetDate = goal.TargetDate,
                AccountId = goal.AccountId,
                Category = goal.Category,
                IsCompleted = goal.IsCompleted,
                CompletedAt = goal.CompletedAt
            };
        }
    }
}
