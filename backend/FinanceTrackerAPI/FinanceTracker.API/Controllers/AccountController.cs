using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly FinanceTrackerDbContext _context;

        public AccountController(ILogger<AccountController> logger, FinanceTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] Account account)
        {
            if (account == null)
                throw new ValidationException("Account cannot be null.");

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account)
        {
            if (account == null)
                throw new ValidationException("Account cannot be null.");

            var existingAccount = await _context.Accounts.FindAsync(id);
            if (existingAccount == null)
                throw new NotFoundException("Account", id);

            existingAccount.Name = account.Name;
            existingAccount.Email = account.Email;
            existingAccount.PasswordHash = account.PasswordHash;

            await _context.SaveChangesAsync();
            return Ok(existingAccount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var existingAccount = await _context.Accounts.FindAsync(id);
            if (existingAccount == null)
                throw new NotFoundException("Account", id);

            _context.Accounts.Remove(existingAccount);
            await _context.SaveChangesAsync();
            return Ok("Account deleted successfully.");
        }
    }
}