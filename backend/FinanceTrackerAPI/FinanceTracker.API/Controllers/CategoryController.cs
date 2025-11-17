using System;
using System.Threading.Tasks;

using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all categories");
                return Problem(title: "GetAllCategories failed", detail: ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategory category)
        {
            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                return Ok(createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category");
                return Problem(title: "CreateCategory failed", detail: ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ExpenseCategory category)
        {
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, category);
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category: {CategoryId}", id);
                return Problem(title: "UpdateCategory failed", detail: ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var deleted = await _categoryService.DeleteCategoryAsync(id);
                if (deleted)
                {
                    return Ok("Category deleted successfully.");
                }
                else
                {
                    return BadRequest("Failed to delete category.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category: {CategoryId}", id);
                return Problem(title: "DeleteCategory failed", detail: ex.Message);
            }
        }
    }
}
