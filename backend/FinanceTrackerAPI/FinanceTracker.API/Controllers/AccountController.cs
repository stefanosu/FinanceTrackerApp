using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
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

        [HttpGet] 
        [Route("all")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpPost]
        [Route("newAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] Account account) 
        {
            await _context.Accounts.AddAsync(account); 
            await _context.SaveChangesAsync();
            return Ok(account);
        }

        [HttpPatch]
        [Route("{id}")] 
        
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account) 
        {
            var existingAccount = await _context.Accounts.FindAsync(id);
            if (existingAccount == null) 
                return NotFound("Account not found.");

                existingAccount.Name = account.Name;
                existingAccount.Email = account.Email;
                existingAccount.PasswordHash = account.PasswordHash;

                await _context.SaveChangesAsync();
                return Ok(existingAccount);
        }
        
        [HttpDelete]
        [Route("{id}")]
        
        public async Task <IActionResult> DeleteAccount(int id) 
        {
            var existingAccount = await _context.Accounts.FindAsync(id);
            if(existingAccount ==  null) 
                return NotFound("Account not found.");

                _context.Accounts.Remove(existingAccount);
                await _context.SaveChangesAsync();
                return Ok(existingAccount);
        }
    }
}