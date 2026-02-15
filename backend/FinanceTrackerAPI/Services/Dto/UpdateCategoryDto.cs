namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for updating an existing category.
    /// All fields optional to support partial updates.
    /// </summary>
    public class UpdateCategoryDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
