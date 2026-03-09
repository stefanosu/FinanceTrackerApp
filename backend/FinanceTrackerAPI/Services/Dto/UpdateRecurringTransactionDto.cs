namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for updating an existing recurring transaction.
    /// All fields optional to support partial updates.
    /// </summary>
    public class UpdateRecurringTransactionDto
    {
        public int? AccountId { get; set; }
        public decimal? Amount { get; set; }
        public string? Type { get; set; }
        public string? Frequency { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
