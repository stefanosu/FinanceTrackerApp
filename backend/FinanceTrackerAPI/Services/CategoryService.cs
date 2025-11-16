using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
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

        public async Task<IEnumerable<ExpenseCategory>> GetAllCategoriesAsync()
        {
            return await _context.ExpenseCategories.ToListAsync();
        }

        public async Task<ExpenseCategory> GetCategoryByIdAsync(int id)
        {
            var category = await _context.ExpenseCategories.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Category", id);

            return category;
        }

        public async Task<ExpenseCategory> CreateCategoryAsync(ExpenseCategory category)
        {
            if (category == null)
                throw new ValidationException("Category cannot be null.");

            await _context.ExpenseCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<ExpenseCategory> UpdateCategoryAsync(int id, ExpenseCategory category)
        {
            if (category == null)
                throw new ValidationException("Category cannot be null.");

            var existingCategory = await _context.ExpenseCategories.FindAsync(id);
            if (existingCategory == null)
                throw new NotFoundException("Category", id);

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();
            return existingCategory;
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
    }
}