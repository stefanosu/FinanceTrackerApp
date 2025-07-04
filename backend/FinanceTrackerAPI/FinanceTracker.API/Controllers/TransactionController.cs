using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase

    {
        private readonly ILogger<TransactionController> _logger;
        private readonly FinanceTrackerDbContext _context;

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var categories = await _context.Transactions.ToListAsync();
            return Ok(categories);
        }

        [HttpPost] 
        [Route("createTransaction")]

        public async Task<IActionResult>CreateTransaction(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return Ok(transaction);

        }

    }
}