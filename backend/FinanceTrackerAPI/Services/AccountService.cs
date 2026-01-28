using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly FinanceTrackerDbContext _context;

        public AccountService(FinanceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountDto dto)
        {
            // Validate input
            if (dto == null)
            {
                throw new ValidationException("Account cannot be null.");
            }
            // Map DTO to entity
            var account = new Account
            {
                id = 0,
                Name = dto.Name,
                Email = "default@email.com",
                AccountType = "Checking",
                Balance = dto.InitialBalance
            };
            // Save to DB
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            // Map entity to DTO and return
            return new AccountDto
            {
                Id = account.id,
                Name = dto.Name,
                Balance = account.Balance,
                CreatedAt = DateTime.UtcNow,
                AccountType = account.AccountType,
                Description = dto.Description ?? string.Empty,
            };
        }

        public async Task<AccountDto> GetAccountByIdAsync(int id)
        {
            // Fetch from DB
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
            // Handle not found
            if (account == null)
            {
                throw new NotFoundException("Account", id);
            }
            // Map to DTO and return
            return new AccountDto
            {
                Id = account.id,
                Name = account.Name,
                Balance = account.Balance,
                CreatedAt = DateTime.UtcNow,
                AccountType = account.AccountType,
                Description = string.Empty
            };
        }

        public async Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountDto dto)
        {
            // Find account
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
            // Validate input
            if (account == null)
            {
                throw new NotFoundException("Account", id);
            }

            if (!string.IsNullOrEmpty(dto.Name))
                account.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Email))
                account.Email = dto.Email;

            //Save changes
            await _context.SaveChangesAsync();

            // Update properties
            return new AccountDto
            {
                Name = account.Name,
                Id = account.id,
                Balance = account.Balance,
                CreatedAt = DateTime.UtcNow,
                Description = string.Empty,
                AccountType = account.AccountType,
            };
        }

        public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return accounts.Select(a => new AccountDto
            {
                Id = a.id,
                Name = a.Name,
                Balance = a.Balance,
                CreatedAt = DateTime.UtcNow,
                AccountType = a.AccountType,
                Description = string.Empty
            });
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.id == id);
            if (account == null)
            {
                throw new NotFoundException("Account", id);
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
