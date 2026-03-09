namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for returning savings goal data.
    /// Includes computed ProgressPercentage.
    /// </summary>
    public class SavingsGoalDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateOnly? TargetDate { get; set; }
        public int? AccountId { get; set; }
        public required string Category { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
