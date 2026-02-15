using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    [EnableRateLimiting("api")] // 100 requests/minute for authenticated users
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            return Ok(transaction);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            var createdTransaction = await _transactionService.CreateTransactionAsync(dto);
            return Ok(createdTransaction);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionDto dto)
        {
            var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, dto);
            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return Ok("Transaction deleted successfully.");
        }
    }
}
