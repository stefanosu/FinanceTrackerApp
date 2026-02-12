namespace FinanceTrackerAPI.Services.Dto.Analytics
{
    public class FinancialSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetSavings { get; set; }
        public decimal SavingsRate { get; set; }
        public decimal TotalBalance { get; set; }
    }

    public class AnalyticsSummaryDto
    {
        public FinancialSummaryDto FinancialSummary { get; set; } = new();
        public List<SpendingByCategoryDto> SpendingByCategory { get; set; } = new();
        public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetSavings { get; set; }
    }

    public class MonthComparisonDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal ThisMonth { get; set; }
        public decimal LastMonth { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
    }
}
