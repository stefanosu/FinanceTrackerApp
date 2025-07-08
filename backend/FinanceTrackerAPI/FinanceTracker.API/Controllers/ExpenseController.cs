using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ILogger<ExpenseController> _logger;
        private readonly FinanceTrackerDbContext _context;

        public ExpenseController(ILogger<ExpenseController> logger, FinanceTrackerDbContext context)
        {
            _logger = logger;
            _context = context; 
        }

        [HttpGet]
        [Route("getExpenses")]
        public async Task <IActionResult> GetExpenses()
        {
            var expenses  = await _context.Expenses.ToListAsync();
            return Ok(expenses);
        }

        [HttpPost]
        [Route("createExpenses")]
        public IActionResult CreateExpense(Expense expense)
        {
            return Ok("Expense created");
        }

        public IActionResult EditExpense(Expense expense) 
        {
            return Ok ("Expense updated");
        }

        public IActionResult RemoveExpense(Expense expense) {
            return Ok("Expense deleted");
        }
    }
}