using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class Transaction : ISoftDeletable
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; }
        public DateOnly Date { get; set; }
        public int CategoryId { get; set; }
        public required string Notes { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Transaction() { }

        public Transaction(int id, int accountId, decimal amount, string type, DateOnly date, int categoryId, string notes)
        {
            Id = id;
            AccountId = accountId;
            Amount = amount;
            Type = type;
            Date = date;
            CategoryId = categoryId;
            Notes = notes;
        }
    }
}
