namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class SavingsGoal : ISoftDeletable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; } = 0;
        public DateOnly? TargetDate { get; set; }
        public int? AccountId { get; set; }
        public required string Category { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public SavingsGoal() { }

        public SavingsGoal(int id, int userId, string name, string description, decimal targetAmount, string category)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            TargetAmount = targetAmount;
            Category = category;
        }

        public decimal ProgressPercentage =>
            TargetAmount > 0 ? Math.Min(100, Math.Round((CurrentAmount / TargetAmount) * 100, 2)) : 0;
    }
}
