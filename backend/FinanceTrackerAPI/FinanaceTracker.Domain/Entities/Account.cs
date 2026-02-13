using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class Account : ISoftDeletable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string AccountType { get; set; }
        public decimal Balance { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Account() { }

        public Account(int id, string name, string email, string accountType, decimal balance = 0)
        {
            Id = id;
            Name = name;
            Email = email;
            AccountType = accountType;
            Balance = balance;
        }
    }
}
