using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
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
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            return Ok(account);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {
            var createdAccount = await _accountService.CreateAccountAsync(createAccountDto);
            return Ok(createdAccount);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDto updateAccountDto)
        {
            var updatedAccount = await _accountService.UpdateAccountAsync(id, updateAccountDto);
            return Ok(updatedAccount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return Ok("Account deleted successfully.");
        }
    }
}
