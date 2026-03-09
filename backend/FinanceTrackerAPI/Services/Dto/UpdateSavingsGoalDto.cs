namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for updating an existing savings goal.
    /// All fields optional to support partial updates.
    /// </summary>
    public class UpdateSavingsGoalDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? TargetAmount { get; set; }
        public DateOnly? TargetDate { get; set; }
        public int? AccountId { get; set; }
        public string? Category { get; set; }
    }
}
