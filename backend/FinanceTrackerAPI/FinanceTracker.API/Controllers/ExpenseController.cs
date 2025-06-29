using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(ILogger<ExpenseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetExpenses()
        {
            return Ok("Expenses");
        }

        [HttpPost]
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