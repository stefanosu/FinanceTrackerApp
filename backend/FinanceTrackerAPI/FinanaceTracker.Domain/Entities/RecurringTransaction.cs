namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class RecurringTransaction : ISoftDeletable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; }
        public required string Frequency { get; set; }
        public required string Description { get; set; }
        public int CategoryId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly NextDueDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public RecurringTransaction() { }

        public RecurringTransaction(int id, int userId, int accountId, decimal amount, string type,
            string frequency, string description, int categoryId, DateOnly startDate, DateOnly nextDueDate)
        {
            Id = id;
            UserId = userId;
            AccountId = accountId;
            Amount = amount;
            Type = type;
            Frequency = frequency;
            Description = description;
            CategoryId = categoryId;
            StartDate = startDate;
            NextDueDate = nextDueDate;
        }
    }
}
