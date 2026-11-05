using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class HouseholdService : IHouseholdService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public HouseholdService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<HouseholdDto?> GetUserHouseholdAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null) return null;

            var membership = await _context.HouseholdMembers
                .Include(hm => hm.Household)
                    .ThenInclude(h => h!.Owner)
                .Include(hm => hm.Household)
                    .ThenInclude(h => h!.Members)
                        .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(hm => hm.UserId == userId.Value);

            if (membership?.Household == null) return null;

            return MapToDto(membership.Household);
        }

        public async Task<HouseholdDto> CreateHouseholdAsync(CreateHouseholdDto dto)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            // Check if user already in a household
            var existingMembership = await _context.HouseholdMembers
                .AnyAsync(hm => hm.UserId == userId.Value);

            if (existingMembership)
                throw new ValidationException("You are already a member of a household. Leave your current household first.");

            var household = new Household
            {
                Name = dto.Name,
                OwnerId = userId.Value,
                InviteCode = GenerateInviteCode(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Households.AddAsync(household);
            await _context.SaveChangesAsync();

            // Add owner as a member
            var ownerMember = new HouseholdMember
            {
                HouseholdId = household.Id,
                UserId = userId.Value,
                Role = HouseholdRole.Owner,
                JoinedAt = DateTime.UtcNow
            };

            await _context.HouseholdMembers.AddAsync(ownerMember);
            await _context.SaveChangesAsync();

            return await GetUserHouseholdAsync() ?? throw new Exception("Failed to create household");
        }

        public async Task<HouseholdDto> UpdateHouseholdAsync(int id, UpdateHouseholdDto dto)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            var household = await _context.Households
                .FirstOrDefaultAsync(h => h.Id == id);

            if (household == null)
                throw new NotFoundException("Household", id);

            if (household.OwnerId != userId.Value)
                throw new ValidationException("Only the household owner can update settings");

            if (!string.IsNullOrEmpty(dto.Name))
                household.Name = dto.Name;

            await _context.SaveChangesAsync();

            return await GetUserHouseholdAsync() ?? throw new Exception("Failed to update household");
        }

        public async Task<bool> DeleteHouseholdAsync(int id)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            var household = await _context.Households
                .FirstOrDefaultAsync(h => h.Id == id);

            if (household == null)
                throw new NotFoundException("Household", id);

            if (household.OwnerId != userId.Value)
                throw new ValidationException("Only the household owner can delete the household");

            // Clear household references from shared expenses
            var sharedExpenses = await _context.Expenses
                .Where(e => e.HouseholdId == id)
                .ToListAsync();

            foreach (var expense in sharedExpenses)
            {
                expense.HouseholdId = null;
            }

            // Soft delete the household
            household.IsDeleted = true;
            household.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HouseholdDto> JoinHouseholdAsync(JoinHouseholdDto dto)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            // Check if user already in a household
            var existingMembership = await _context.HouseholdMembers
                .AnyAsync(hm => hm.UserId == userId.Value);

            if (existingMembership)
                throw new ValidationException("You are already a member of a household. Leave your current household first.");

            var household = await _context.Households
                .FirstOrDefaultAsync(h => h.InviteCode == dto.InviteCode.ToUpper());

            if (household == null)
                throw new ValidationException("Invalid invite code");

            var member = new HouseholdMember
            {
                HouseholdId = household.Id,
                UserId = userId.Value,
                Role = HouseholdRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            await _context.HouseholdMembers.AddAsync(member);
            await _context.SaveChangesAsync();

            return await GetUserHouseholdAsync() ?? throw new Exception("Failed to join household");
        }

        public async Task<bool> LeaveHouseholdAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            var membership = await _context.HouseholdMembers
                .Include(hm => hm.Household)
                .FirstOrDefaultAsync(hm => hm.UserId == userId.Value);

            if (membership == null)
                throw new ValidationException("You are not a member of any household");

            // Owner cannot leave - must delete or transfer ownership
            if (membership.Role == HouseholdRole.Owner)
                throw new ValidationException("Household owners cannot leave. Delete the household or transfer ownership first.");

            // Clear household references from user's shared expenses
            var userSharedExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.HouseholdId == membership.HouseholdId)
                .ToListAsync();

            foreach (var expense in userSharedExpenses)
            {
                expense.HouseholdId = null;
            }

            _context.HouseholdMembers.Remove(membership);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> RegenerateInviteCodeAsync(int householdId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            var household = await _context.Households
                .FirstOrDefaultAsync(h => h.Id == householdId);

            if (household == null)
                throw new NotFoundException("Household", householdId);

            if (household.OwnerId != userId.Value)
                throw new ValidationException("Only the household owner can regenerate the invite code");

            household.InviteCode = GenerateInviteCode();
            await _context.SaveChangesAsync();

            return household.InviteCode;
        }

        public async Task<bool> RemoveMemberAsync(int householdId, int memberId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                throw new ValidationException("User not authenticated");

            var household = await _context.Households
                .FirstOrDefaultAsync(h => h.Id == householdId);

            if (household == null)
                throw new NotFoundException("Household", householdId);

            if (household.OwnerId != userId.Value)
                throw new ValidationException("Only the household owner can remove members");

            if (memberId == userId.Value)
                throw new ValidationException("You cannot remove yourself. Use leave or delete household instead.");

            var membership = await _context.HouseholdMembers
                .FirstOrDefaultAsync(hm => hm.HouseholdId == householdId && hm.UserId == memberId);

            if (membership == null)
                throw new ValidationException("User is not a member of this household");

            // Clear household references from removed member's shared expenses
            var memberSharedExpenses = await _context.Expenses
                .Where(e => e.UserId == memberId && e.HouseholdId == householdId)
                .ToListAsync();

            foreach (var expense in memberSharedExpenses)
            {
                expense.HouseholdId = null;
            }

            _context.HouseholdMembers.Remove(membership);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<HouseholdSummaryDto?> GetHouseholdSummaryAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null) return null;

            var membership = await _context.HouseholdMembers
                .Include(hm => hm.Household)
                    .ThenInclude(h => h!.Members)
                .FirstOrDefaultAsync(hm => hm.UserId == userId.Value);

            if (membership?.Household == null) return null;

            var householdId = membership.HouseholdId;
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var totalSharedExpenses = await _context.Expenses
                .Where(e => e.HouseholdId == householdId)
                .SumAsync(e => e.Amount);

            var thisMonthSharedExpenses = await _context.Expenses
                .Where(e => e.HouseholdId == householdId && e.Date >= startOfMonth)
                .SumAsync(e => e.Amount);

            return new HouseholdSummaryDto
            {
                Id = membership.Household.Id,
                Name = membership.Household.Name,
                MemberCount = membership.Household.Members.Count,
                TotalSharedExpenses = totalSharedExpenses,
                ThisMonthSharedExpenses = thisMonthSharedExpenses
            };
        }

        private static HouseholdDto MapToDto(Household household)
        {
            return new HouseholdDto
            {
                Id = household.Id,
                Name = household.Name,
                InviteCode = household.InviteCode,
                OwnerId = household.OwnerId,
                OwnerName = household.Owner != null
                    ? $"{household.Owner.FirstName} {household.Owner.LastName}"
                    : string.Empty,
                CreatedAt = household.CreatedAt,
                Members = household.Members.Select(m => new HouseholdMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    FirstName = m.User?.FirstName ?? string.Empty,
                    LastName = m.User?.LastName ?? string.Empty,
                    Email = m.User?.Email ?? string.Empty,
                    Role = m.Role.ToString(),
                    JoinedAt = m.JoinedAt
                }).ToList()
            };
        }

        private static string GenerateInviteCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
