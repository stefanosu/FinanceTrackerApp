using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services.Dto.Analytics;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AnalyticsService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var financialSummary = await GetFinancialSummaryAsync(startDate, endDate);
            var spendingByCategory = await GetSpendingByCategoryAsync(startDate, endDate);
            var monthlyTrend = await GetMonthlyTrendAsync(6);

            return new AnalyticsSummaryDto
            {
                FinancialSummary = financialSummary,
                SpendingByCategory = spendingByCategory,
                MonthlyTrend = monthlyTrend
            };
        }

        public async Task<List<SpendingByCategoryDto>> GetSpendingByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new List<SpendingByCategoryDto>();

            var query = _context.Expenses.Where(e => e.UserId == userId.Value);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            var expenses = await query.ToListAsync();
            var totalExpenses = expenses.Sum(e => e.Amount);

            if (totalExpenses == 0)
                return new List<SpendingByCategoryDto>();

            var categoryGroups = expenses
                .GroupBy(e => e.Category)
                .Select(g => new SpendingByCategoryDto
                {
                    Category = g.Key,
                    Amount = g.Sum(e => e.Amount),
                    Percentage = Math.Round(g.Sum(e => e.Amount) / totalExpenses * 100, 2)
                })
                .OrderByDescending(c => c.Amount)
                .ToList();

            return categoryGroups;
        }

        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new FinancialSummaryDto();

            // Get total balance from accounts
            var totalBalance = await _context.Accounts
                .SumAsync(a => a.Balance);

            // Get expenses within date range
            var expenseQuery = _context.Expenses.Where(e => e.UserId == userId.Value);
            if (startDate.HasValue)
                expenseQuery = expenseQuery.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                expenseQuery = expenseQuery.Where(e => e.Date <= endDate.Value);

            var totalExpenses = await expenseQuery.SumAsync(e => e.Amount);

            // Get income from transactions (type = "Income" or "Deposit")
            var transactionQuery = _context.Transactions.AsQueryable();
            if (startDate.HasValue)
            {
                var startDateOnly = DateOnly.FromDateTime(startDate.Value);
                transactionQuery = transactionQuery.Where(t => t.Date >= startDateOnly);
            }
            if (endDate.HasValue)
            {
                var endDateOnly = DateOnly.FromDateTime(endDate.Value);
                transactionQuery = transactionQuery.Where(t => t.Date <= endDateOnly);
            }

            var incomeTransactions = await transactionQuery
                .Where(t => t.Type == "Income" || t.Type == "Deposit")
                .SumAsync(t => t.Amount);

            var netSavings = incomeTransactions - totalExpenses;
            var savingsRate = incomeTransactions > 0
                ? Math.Round((netSavings / incomeTransactions) * 100, 2)
                : 0;

            return new FinancialSummaryDto
            {
                TotalIncome = incomeTransactions,
                TotalExpenses = totalExpenses,
                NetSavings = netSavings,
                SavingsRate = savingsRate,
                TotalBalance = totalBalance
            };
        }

        public async Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(int months = 6)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new List<MonthlyTrendDto>();

            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months + 1).Date;
            startDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= startDate)
                .ToListAsync();

            var startDateOnly = DateOnly.FromDateTime(startDate);
            var transactions = await _context.Transactions
                .Where(t => t.Date >= startDateOnly && (t.Type == "Income" || t.Type == "Deposit"))
                .ToListAsync();

            var monthlyData = new List<MonthlyTrendDto>();

            for (int i = 0; i < months; i++)
            {
                var monthStart = startDate.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var monthStartOnly = DateOnly.FromDateTime(monthStart);
                var monthEndOnly = DateOnly.FromDateTime(monthEnd);

                var monthExpenses = expenses
                    .Where(e => e.Date >= monthStart && e.Date <= monthEnd)
                    .Sum(e => e.Amount);

                var monthIncome = transactions
                    .Where(t => t.Date >= monthStartOnly && t.Date <= monthEndOnly)
                    .Sum(t => t.Amount);

                monthlyData.Add(new MonthlyTrendDto
                {
                    Month = monthStart.ToString("MMM"),
                    Year = monthStart.Year,
                    Income = monthIncome,
                    Expenses = monthExpenses,
                    NetSavings = monthIncome - monthExpenses
                });
            }

            return monthlyData;
        }

        public async Task<List<MonthComparisonDto>> GetMonthComparisonAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new List<MonthComparisonDto>();

            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var lastMonthEnd = thisMonthStart.AddDays(-1);

            // Get this month's expenses by category
            var thisMonthExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= thisMonthStart)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(e => e.Amount) })
                .ToListAsync();

            // Get last month's expenses by category
            var lastMonthExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= lastMonthStart && e.Date < thisMonthStart)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(e => e.Amount) })
                .ToListAsync();

            // Combine all categories
            var allCategories = thisMonthExpenses.Select(e => e.Category)
                .Union(lastMonthExpenses.Select(e => e.Category))
                .Distinct();

            var comparison = allCategories.Select(category =>
            {
                var thisMonth = thisMonthExpenses.FirstOrDefault(e => e.Category == category)?.Amount ?? 0;
                var lastMonth = lastMonthExpenses.FirstOrDefault(e => e.Category == category)?.Amount ?? 0;
                var change = thisMonth - lastMonth;
                var changePercent = lastMonth > 0 ? Math.Round((change / lastMonth) * 100, 1) : (thisMonth > 0 ? 100 : 0);

                return new MonthComparisonDto
                {
                    Category = category,
                    ThisMonth = thisMonth,
                    LastMonth = lastMonth,
                    Change = change,
                    ChangePercent = changePercent
                };
            })
            .OrderByDescending(c => c.ThisMonth)
            .Take(6)
            .ToList();

            return comparison;
        }
    }
}
