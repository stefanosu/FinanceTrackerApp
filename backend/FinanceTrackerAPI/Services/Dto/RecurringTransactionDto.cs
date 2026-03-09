namespace FinanceTrackerAPI.Services.Dtos
{
    /// <summary>
    /// DTO for returning recurring transaction data.
    /// Excludes soft delete fields (IsDeleted, DeletedAt).
    /// </summary>
    public class RecurringTransactionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; }
        public required string Frequency { get; set; }
        public required string Description { get; set; }
        public int CategoryId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly NextDueDate { get; set; }
        public bool IsActive { get; set; }
    }
}
