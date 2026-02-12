namespace FinanceTrackerAPI.Services.Dto.AI
{
    public class FinancialContextDto
    {
        public decimal TotalBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal SavingsRate { get; set; }
        public List<CategorySpending> TopCategories { get; set; } = new();
        public List<RecentExpense> RecentExpenses { get; set; } = new();
        public string SpendingTrend { get; set; } = string.Empty;
    }

    public class CategorySpending
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RecentExpense
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
