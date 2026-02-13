using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AccountService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        private async Task<string?> GetCurrentUserEmailAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null) return null;

            var user = await _context.Users.FindAsync(userId.Value);
            return user?.Email;
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountDto dto)
        {
            // Validate input
            if (dto == null)
            {
                throw new ValidationException("Account cannot be null.");
            }

            var userEmail = await GetCurrentUserEmailAsync();
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new ValidationException("User not authenticated.");
            }

            // Map DTO to entity
            var account = new Account
            {
                Id = 0,
                Name = dto.Name,
                Email = userEmail,
                AccountType = "Checking",
                Balance = dto.InitialBalance
            };
            // Save to DB
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            // Map entity to DTO and return
            return new AccountDto
            {
                Id = account.Id,
                Name = dto.Name,
                Balance = account.Balance,
                CreatedAt = DateTime.UtcNow,
                AccountType = account.AccountType,
                Description = dto.Description ?? string.Empty,
            };
        }

        public async Task<AccountDto> GetAccountByIdAsync(int id)
        {
            var userEmail = await GetCurrentUserEmailAsync();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            // Verify account exists and belongs to current user
            if (account == null || (userEmail != null && !account.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new NotFoundException("Account", id);
            }

            return new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Balance = account.Balance,
                CreatedAt = DateTime.UtcNow,
                AccountType = account.AccountType,
                Description = string.Empty
            };
        }

        public async Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountDto dto)
        {
            var userEmail = await GetCurrentUserEmailAsync();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            // Verify account exists and belongs to current user
            if (account == null || (userEmail != null && !account.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new NotFoundException("Account", id);
            }

            if (!string.IsNullOrEmpty(dto.Name))
                account.Name = dto.Name;

            // Don't allow changing email - it links account to user

            await _context.SaveChangesAsync();

            return new AccountDto
            {
                Name = account.Name,
                Id = account.Id,
                Balance = account.Balance,
                CreatedAt = DateTime.UtcNow,
                Description = string.Empty,
                AccountType = account.AccountType,
            };
        }

        public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
        {
            var userEmail = await GetCurrentUserEmailAsync();
            if (string.IsNullOrEmpty(userEmail))
                return Enumerable.Empty<AccountDto>();

            var accounts = await _context.Accounts
                .Where(a => a.Email.ToLower() == userEmail.ToLower())
                .ToListAsync();

            return accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                Balance = a.Balance,
                CreatedAt = DateTime.UtcNow,
                AccountType = a.AccountType,
                Description = string.Empty
            });
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var userEmail = await GetCurrentUserEmailAsync();
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

            // Verify account exists and belongs to current user
            if (account == null || (userEmail != null && !account.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase)))
            {
                throw new NotFoundException("Account", id);
            }

            // Soft delete: mark as deleted instead of removing
            account.IsDeleted = true;
            account.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
