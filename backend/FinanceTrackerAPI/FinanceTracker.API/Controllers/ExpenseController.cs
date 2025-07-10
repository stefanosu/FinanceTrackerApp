using System;
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
    public class ExpenseController : ControllerBase
    {
        private readonly ILogger<ExpenseController> _logger;
        private readonly FinanceTrackerDbContext _context;

        public ExpenseController(ILogger<ExpenseController> logger, FinanceTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]

        public async Task<IActionResult> GetAllExpenses()
        {
            var expenses = await _context.Expenses.ToListAsync();
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                throw new NotFoundException("Expense", id);

            return Ok(expense);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
        {
            if (expense == null)
                throw new ValidationException("Expense cannot be null.");

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
            return Ok(expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense expense)
        {
            if (expense == null)
                throw new ValidationException("Expense cannot be null.");

            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
                throw new NotFoundException("Expense", id);

            existingExpense.Name = expense.Name;
            existingExpense.Description = expense.Description;
            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Category = expense.Category;
            existingExpense.SubCategory = expense.SubCategory;
            existingExpense.PaymentMethod = expense.PaymentMethod;
            existingExpense.Notes = expense.Notes;

            await _context.SaveChangesAsync();
            return Ok(existingExpense);
        }

        [HttpDelete("{id}")]
        [Route("")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
                throw new NotFoundException("Expense", id);

            _context.Expenses.Remove(existingExpense);
            await _context.SaveChangesAsync();
            return Ok("Expense deleted successfully.");
        }
    }
}