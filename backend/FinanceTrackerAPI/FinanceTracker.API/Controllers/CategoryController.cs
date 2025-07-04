using System;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

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

        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.ExpenseCategories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost] 
        [Route("createCategory")]

        public async Task<IActionResult>CreateCategory(ExpenseCategory category)
        {
            await _context.ExpenseCategories.AddAsync(category); 
            await _context.SaveChangesAsync();
            return Ok(category);

        }

        [HttpPatch] 
        [Route("{id}")]

        public async Task<IActionResult> UpdateCategory( int id, [FromBody] ExpenseCategory category)
        {
            var existingCategory = await _context.ExpenseCategories.FindAsync(id); 
            if (existingCategory == null)
                return NotFound("Category not found."); 

                existingCategory.Name = category.Name; 
                existingCategory.Description = category.Description;
                
                await _context.SaveChangesAsync();
                return Ok(existingCategory);
        }

        [HttpDelete]
        [Route("{id}")]

        public async Task<IActionResult> DeleteCategory(int id) 
        {
            var existingCategory = await _context.ExpenseCategories.FindAsync(id);
            if(existingCategory == null) 
                return NotFound("Category not found.");

                _context.ExpenseCategories.Remove(existingCategory);
                await _context.SaveChangesAsync();
                return Ok("Category deleted");
        }
    }
}