namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for adding a contribution to a savings goal.
    /// </summary>
    public class ContributeToGoalDto
    {
        public required decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}
