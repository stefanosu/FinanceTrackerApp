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
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly FinanceTrackerDbContext _context;

        public CategoryController(ILogger<CategoryController> logger, FinanceTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.ExpenseCategories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategory category)
        {
            if (category == null)
                throw new ValidationException("Category cannot be null.");

            await _context.ExpenseCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ExpenseCategory category)
        {
            if (category == null)
                throw new ValidationException("Category cannot be null.");

            var existingCategory = await _context.ExpenseCategories.FindAsync(id);
            if (existingCategory == null)
                throw new NotFoundException("Category", id);

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();
            return Ok(existingCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var existingCategory = await _context.ExpenseCategories.FindAsync(id);
            if (existingCategory == null)
                throw new NotFoundException("Category", id);

            _context.ExpenseCategories.Remove(existingCategory);
            await _context.SaveChangesAsync();
            return Ok("Category deleted successfully.");
        }
    }
}