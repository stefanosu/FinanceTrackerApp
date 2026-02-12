using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string AccountType { get; set; }
        public decimal Balance { get; set; }

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
