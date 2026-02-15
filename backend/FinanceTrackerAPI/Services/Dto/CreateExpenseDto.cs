namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for creating a new expense.
    /// Excludes Id (server-generated), UserId (from auth), and soft-delete fields.
    /// </summary>
    public class CreateExpenseDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime Date { get; set; }
        public required string Category { get; set; }
        public string? SubCategory { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}
