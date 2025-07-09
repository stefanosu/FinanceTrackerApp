using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly FinanceTrackerDbContext _context;

        public TransactionController(ILogger<TransactionController> logger, FinanceTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
                throw new ValidationException("Transaction cannot be null.");

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction transaction)
        {
            if (transaction == null)
                throw new ValidationException("Transaction cannot be null.");

            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
                throw new NotFoundException("Transaction", id);

            // Update transaction properties here
            // existingTransaction.Property = transaction.Property;

            await _context.SaveChangesAsync();
            return Ok(existingTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
                throw new NotFoundException("Transaction", id);

            _context.Transactions.Remove(existingTransaction);
            await _context.SaveChangesAsync();
            return Ok("Transaction deleted successfully.");
        }
    }
}