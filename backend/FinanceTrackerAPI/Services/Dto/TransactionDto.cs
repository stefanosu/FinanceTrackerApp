namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for returning transaction data.
    /// Excludes soft delete fields (IsDeleted, DeletedAt).
    /// </summary>
    public class TransactionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; }
        public DateOnly Date { get; set; }
        public int CategoryId { get; set; }
        public string? Notes { get; set; }
    }
}
