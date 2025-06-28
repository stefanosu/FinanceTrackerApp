
namespace FinanceTrackerAPI.FinanaceTracker.Domain.Entities
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