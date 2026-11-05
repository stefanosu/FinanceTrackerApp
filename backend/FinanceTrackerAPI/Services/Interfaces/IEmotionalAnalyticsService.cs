using FinanceTrackerAPI.Services.Dto.Analytics;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IEmotionalAnalyticsService
    {
        Task<EmotionalAnalyticsSummaryDto> GetEmotionalAnalyticsSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<SpendingByMoodDto>> GetSpendingByMoodAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MoodTrendDto>> GetMoodTrendAsync(int months = 6);
        Task<EmotionalInsightsDto> GetEmotionalInsightsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
