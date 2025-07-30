using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Services.Interfaces;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services.Dtos;

namespace backend.Services
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
            if(dto == null) {
                throw new ArgumentNullException("Account info not valid.");
            }
            // Map DTO to entity
            var account = new Account {
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
            return new AccountDto {
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
            // Handle not found
            // Map to DTO and return
            return await Task.FromResult<AccountDto>(null);
        }

        // ... other methods (Update, Delete, etc.)
    }
}