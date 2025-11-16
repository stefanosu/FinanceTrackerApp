using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.Services.Interfaces;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all transactions");
                return Problem(title: "GetAllTransactions failed", detail: ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            try
            {
                var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);
                return Ok(createdTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create transaction");
                return Problem(title: "CreateTransaction failed", detail: ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction transaction)
        {
            try
            {
                var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transaction);
                return Ok(updatedTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update transaction: {TransactionId}", id);
                return Problem(title: "UpdateTransaction failed", detail: ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var deleted = await _transactionService.DeleteTransactionAsync(id);
                if (deleted)
                {
                    return Ok("Transaction deleted successfully.");
                }
                else
                {
                    return BadRequest("Failed to delete transaction.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete transaction: {TransactionId}", id);
                return Problem(title: "DeleteTransaction failed", detail: ex.Message);
            }
        }
    }
}
