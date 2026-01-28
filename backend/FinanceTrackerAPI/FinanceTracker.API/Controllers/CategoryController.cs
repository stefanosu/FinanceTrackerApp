using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategory category)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(category);
            return Ok(createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ExpenseCategory category)
        {
            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, category);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok("Category deleted successfully.");
        }
    }
}
