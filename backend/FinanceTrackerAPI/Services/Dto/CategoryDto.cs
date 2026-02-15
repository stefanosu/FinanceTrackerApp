namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for returning category data to clients.
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
