using FinanceTrackerAPI.Services.Dto.Analytics;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<SpendingByCategoryDto>> GetSpendingByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MonthlyTrendDto>> GetMonthlyTrendAsync(int months = 6);
        Task<List<MonthComparisonDto>> GetMonthComparisonAsync();
    }
}
