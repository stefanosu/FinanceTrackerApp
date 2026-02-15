namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for updating an existing transaction.
    /// All fields optional to support partial updates.
    /// </summary>
    public class UpdateTransactionDto
    {
        public int? AccountId { get; set; }
        public decimal? Amount { get; set; }
        public string? Type { get; set; }
        public DateOnly? Date { get; set; }
        public int? CategoryId { get; set; }
        public string? Notes { get; set; }
    }
}
