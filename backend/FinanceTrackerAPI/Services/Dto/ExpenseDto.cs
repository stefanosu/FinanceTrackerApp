namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for returning expense data to clients.
    /// Excludes sensitive/internal fields like UserId, IsDeleted, DeletedAt.
    /// </summary>
    public class ExpenseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Category { get; set; }
        public string? SubCategory { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}
