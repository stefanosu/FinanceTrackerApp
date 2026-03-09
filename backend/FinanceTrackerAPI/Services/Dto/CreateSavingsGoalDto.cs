namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for creating a new savings goal.
    /// </summary>
    public class CreateSavingsGoalDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal TargetAmount { get; set; }
        public decimal? InitialAmount { get; set; }
        public DateOnly? TargetDate { get; set; }
        public int? AccountId { get; set; }
        public required string Category { get; set; }
    }
}
