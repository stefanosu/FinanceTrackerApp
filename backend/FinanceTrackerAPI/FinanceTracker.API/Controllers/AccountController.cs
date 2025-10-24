using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using FinanceTrackerAPI.Services.Interfaces;
using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.FinanceTracker.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService) : base(logger)
        {
            _accountService = accountService;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("pong");

        [HttpGet("health/db")]
        public IActionResult DbHealth()
        {
            // Note: This health check would need to be implemented differently
            // since we no longer have direct access to DbContext
            // Consider moving this to a dedicated health check controller
            return Ok(new { status = "healthy", message = "Account service is running" });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccounts()
        {
            return await HandleServiceResult(() => _accountService.GetAllAccountsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            if(!ValidateId(id)) 
            {
                return BadRequest($"Invalid account ID: {id}. ID must be a positive integer.");
            }
            
            return await HandleServiceResult(() => _accountService.GetAccountByIdAsync(id));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {

            if (createAccountDto == null)
                throw new ValidationException("Account cannot be null.");

            return await HandleServiceResult(() => _accountService.CreateAccountAsync(createAccountDto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDto updateAccountDto)
        {
            if (updateAccountDto == null)
                throw new ValidationException("Account cannot be null.");

            if(!ValidateId(id)) 
            {
                return BadRequest($"Invalid account ID: {id}. ID must be a positive integer.");
            }

            return await HandleServiceResult(() => _accountService.UpdateAccountAsync(id, updateAccountDto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if(!ValidateId(id)) 
            {
                return BadRequest($"Invalid account ID: {id}. ID must be a positive integer.");
            }

            return await HandleServiceResult(() => _accountService.DeleteAccountAsync(id));
        }
    }
}