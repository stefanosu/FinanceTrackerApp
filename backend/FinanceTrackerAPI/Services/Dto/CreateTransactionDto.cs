namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for creating a new transaction.
    /// Excludes Id (server-generated) and soft delete fields.
    /// </summary>
    public class CreateTransactionDto
    {
        public required int AccountId { get; set; }
        public required decimal Amount { get; set; }
        public required string Type { get; set; }
        public required DateOnly Date { get; set; }
        public required int CategoryId { get; set; }
        public string? Notes { get; set; }
    }
}
