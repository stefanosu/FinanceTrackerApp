using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
