namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for updating an existing expense.
    /// All fields are optional to support partial updates.
    /// </summary>
    public class UpdateExpenseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }
}
