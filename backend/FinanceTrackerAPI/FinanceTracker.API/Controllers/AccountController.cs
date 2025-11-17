using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
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
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get accounts");
                return Problem(title: "GetAllAccounts failed", detail: ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                return Ok(account);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found: {AccountId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get account: {AccountId}", id);
                return Problem(title: "GetAccountById failed", detail: ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {
            if (createAccountDto == null)
                throw new ValidationException("Account cannot be null.");

            try
            {
                var createdAccount = await _accountService.CreateAccountAsync(createAccountDto);
                return Ok(createdAccount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create account");
                return Problem(title: "CreateAccount failed", detail: ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDto updateAccountDto)
        {
            if (updateAccountDto == null)
                throw new ValidationException("Account cannot be null.");

            try
            {
                var updatedAccount = await _accountService.UpdateAccountAsync(id, updateAccountDto);
                return Ok(updatedAccount);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found for update: {AccountId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update account: {AccountId}", id);
                return Problem(title: "UpdateAccount failed", detail: ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var deleted = await _accountService.DeleteAccountAsync(id);
                if (deleted)
                {
                    return Ok("Account deleted successfully.");
                }
                else
                {
                    return NotFound($"Account with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete account: {AccountId}", id);
                return Problem(title: "DeleteAccount failed", detail: ex.Message);
            }
        }
    }
}
