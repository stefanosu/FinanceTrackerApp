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

        public async Task void UpdateCategory(ExpenseCategory category)
        {
            
        }
    }
}