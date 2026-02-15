using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly FinanceTrackerDbContext _context;

        public CategoryService(FinanceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.ExpenseCategories.ToListAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.ExpenseCategories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Category", id);

            return MapToDto(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (dto == null)
                throw new ValidationException("Category cannot be null.");

            var category = new ExpenseCategory
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty
            };

            await _context.ExpenseCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            if (dto == null)
                throw new ValidationException("Category cannot be null.");

            var existingCategory = await _context.ExpenseCategories.FindAsync(id);
            if (existingCategory == null)
                throw new NotFoundException("Category", id);

            // Update only provided fields (partial update)
            if (!string.IsNullOrEmpty(dto.Name))
                existingCategory.Name = dto.Name;
            if (dto.Description != null)
                existingCategory.Description = dto.Description;

            await _context.SaveChangesAsync();
            return MapToDto(existingCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existingCategory = await _context.ExpenseCategories.FindAsync(id);
            if (existingCategory == null)
                throw new NotFoundException("Category", id);

            _context.ExpenseCategories.Remove(existingCategory);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Maps an ExpenseCategory entity to CategoryDto.
        /// </summary>
        private static CategoryDto MapToDto(ExpenseCategory category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
