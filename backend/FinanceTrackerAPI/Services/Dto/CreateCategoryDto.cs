namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for creating a new category.
    /// Excludes Id (server-generated).
    /// </summary>
    public class CreateCategoryDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
