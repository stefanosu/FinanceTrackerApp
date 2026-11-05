using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.Services.Dto.Analytics;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class EmotionalAnalyticsService : IEmotionalAnalyticsService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public EmotionalAnalyticsService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<EmotionalAnalyticsSummaryDto> GetEmotionalAnalyticsSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var spendingByMood = await GetSpendingByMoodAsync(startDate, endDate);
            var moodTrend = await GetMoodTrendAsync(6);
            var insights = await GetEmotionalInsightsAsync(startDate, endDate);

            return new EmotionalAnalyticsSummaryDto
            {
                SpendingByMood = spendingByMood,
                MoodTrend = moodTrend,
                Insights = insights
            };
        }

        public async Task<List<SpendingByMoodDto>> GetSpendingByMoodAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new List<SpendingByMoodDto>();

            var query = _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Mood != null);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            var expenses = await query.ToListAsync();
            var totalAmount = expenses.Sum(e => e.Amount);

            if (totalAmount == 0)
                return new List<SpendingByMoodDto>();

            var moodGroups = expenses
                .GroupBy(e => e.Mood!.Value)
                .Select(g => new SpendingByMoodDto
                {
                    Mood = g.Key.ToString(),
                    Amount = g.Sum(e => e.Amount),
                    Percentage = Math.Round(g.Sum(e => e.Amount) / totalAmount * 100, 2),
                    TransactionCount = g.Count(),
                    AverageTransaction = Math.Round(g.Average(e => e.Amount), 2)
                })
                .OrderByDescending(m => m.Amount)
                .ToList();

            return moodGroups;
        }

        public async Task<List<MoodTrendDto>> GetMoodTrendAsync(int months = 6)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new List<MoodTrendDto>();

            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months + 1).Date;
            startDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= startDate && e.Mood != null)
                .ToListAsync();

            var monthlyData = new List<MoodTrendDto>();

            for (int i = 0; i < months; i++)
            {
                var monthStart = startDate.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthExpenses = expenses
                    .Where(e => e.Date >= monthStart && e.Date <= monthEnd)
                    .ToList();

                var moodBreakdown = monthExpenses
                    .GroupBy(e => e.Mood!.Value.ToString())
                    .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

                var dominantMood = moodBreakdown.Any()
                    ? moodBreakdown.OrderByDescending(m => m.Value).First().Key
                    : "None";

                monthlyData.Add(new MoodTrendDto
                {
                    Month = monthStart.ToString("MMM"),
                    Year = monthStart.Year,
                    DominantMood = dominantMood,
                    TotalSpending = monthExpenses.Sum(e => e.Amount),
                    MoodBreakdown = moodBreakdown
                });
            }

            return monthlyData;
        }

        public async Task<EmotionalInsightsDto> GetEmotionalInsightsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new EmotionalInsightsDto();

            var query = _context.Expenses.Where(e => e.UserId == userId.Value);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);

            var allExpenses = await query.ToListAsync();
            var moodExpenses = allExpenses.Where(e => e.Mood != null).ToList();

            if (!moodExpenses.Any())
            {
                return new EmotionalInsightsDto
                {
                    PercentageWithMoodTracked = 0
                };
            }

            var moodGroups = moodExpenses
                .GroupBy(e => e.Mood!.Value)
                .Select(g => new
                {
                    Mood = g.Key,
                    Amount = g.Sum(e => e.Amount),
                    Count = g.Count()
                })
                .ToList();

            var highestSpending = moodGroups.OrderByDescending(m => m.Amount).First();
            var mostFrequent = moodGroups.OrderByDescending(m => m.Count).First();

            return new EmotionalInsightsDto
            {
                HighestSpendingMood = highestSpending.Mood.ToString(),
                HighestSpendingMoodAmount = highestSpending.Amount,
                MostFrequentMood = mostFrequent.Mood.ToString(),
                MostFrequentMoodCount = mostFrequent.Count,
                AverageEmotionalSpending = Math.Round(moodExpenses.Average(e => e.Amount), 2),
                PercentageWithMoodTracked = allExpenses.Any()
                    ? Math.Round((decimal)moodExpenses.Count / allExpenses.Count * 100, 2)
                    : 0
            };
        }
    }
}
