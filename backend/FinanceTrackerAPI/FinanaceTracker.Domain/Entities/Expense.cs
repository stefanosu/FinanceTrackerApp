using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime Date { get; set; }
        public required string Category { get; set; }
        public required string SubCategory { get; set; }
        public required string PaymentMethod { get; set; }
        public required string Notes { get; set; }
        public required int UserId { get; set; }

        public Expense() { }

        public Expense(int id, string name, string description, decimal amount, DateTime date, string category, string subCategory, string paymentMethod, string notes, int userId)
        {
            Id = id;
            Name = name;
            Description = description;
            Amount = amount;
            Date = date;
            Category = category;
            SubCategory = subCategory;
            PaymentMethod = paymentMethod;
            Notes = notes;
            UserId = userId;
        }
    }

    public class ExpenseCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class ExpenseSubCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int CategoryId { get; set; }
    }

    public class ExpensePaymentMethod
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}