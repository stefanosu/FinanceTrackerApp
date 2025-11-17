using System;
using System.Threading.Tasks;
using backend.Services.Interfaces;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    public class ExpenseController : BaseController
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(ILogger<ExpenseController> logger, IExpenseService expenseService)
            : base(logger)
        {
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
            // Input validation
            if (!ValidateId(id))
            {
                return BadRequest($"Invalid expense ID: {id}. ID must be a positive integer.");
            }

            return await HandleServiceResult(() => _expenseService.GetExpenseByIdAsync(id));
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
