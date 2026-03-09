using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("api")]
    public class RecurringTransactionController : ControllerBase
    {
        private readonly IRecurringTransactionService _recurringTransactionService;

        public RecurringTransactionController(IRecurringTransactionService recurringTransactionService)
        {
            _recurringTransactionService = recurringTransactionService;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<RecurringTransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRecurringTransactions()
        {
            var recurringTransactions = await _recurringTransactionService.GetAllRecurringTransactionsAsync();
            return Ok(recurringTransactions);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RecurringTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecurringTransactionById(int id)
        {
            var recurringTransaction = await _recurringTransactionService.GetRecurringTransactionByIdAsync(id);
            return Ok(recurringTransaction);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(RecurringTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRecurringTransaction([FromBody] CreateRecurringTransactionDto dto)
        {
            var created = await _recurringTransactionService.CreateRecurringTransactionAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RecurringTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRecurringTransaction(int id, [FromBody] UpdateRecurringTransactionDto dto)
        {
            var updated = await _recurringTransactionService.UpdateRecurringTransactionAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecurringTransaction(int id)
        {
            await _recurringTransactionService.DeleteRecurringTransactionAsync(id);
            return Ok("Recurring transaction deleted successfully.");
        }

        [HttpPost("process-due")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessDueTransactions()
        {
            var createdTransactions = await _recurringTransactionService.ProcessDueTransactionsAsync();
            return Ok(createdTransactions);
        }
    }
}
