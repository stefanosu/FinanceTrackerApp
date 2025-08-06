using System;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using backend.Services.Interfaces;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ILogger<ExpenseController> _logger;
        private readonly IExpenseService _expenseService;

        public ExpenseController(ILogger<ExpenseController> logger, IExpenseService expenseService)
        {
            _logger = logger;
            _expenseService = expenseService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllExpenses()
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            return Ok(expense);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
        {
            var createdExpense = await _expenseService.CreateExpenseAsync(expense);
            return Ok(createdExpense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense expense)
        {
            var updatedExpense = await _expenseService.UpdateExpenseAsync(id, expense);
            return Ok(updatedExpense);
        }

        [HttpDelete("{id}")]
        [Route("")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            await _expenseService.DeleteExpenseAsync(id);
            return Ok("Expense deleted successfully.");
        }
    }
}