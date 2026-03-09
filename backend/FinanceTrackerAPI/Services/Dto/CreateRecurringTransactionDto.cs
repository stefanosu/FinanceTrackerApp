namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for creating a new recurring transaction.
    /// Excludes Id (server-generated) and soft delete fields.
    /// </summary>
    public class CreateRecurringTransactionDto
    {
        public required int AccountId { get; set; }
        public required decimal Amount { get; set; }
        public required string Type { get; set; }
        public required string Frequency { get; set; }
        public required string Description { get; set; }
        public required int CategoryId { get; set; }
        public required DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
