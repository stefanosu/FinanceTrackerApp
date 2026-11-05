namespace FinanceTrackerAPI.Services.Dto.Analytics
{
    public class EmotionalAnalyticsSummaryDto
    {
        public List<SpendingByMoodDto> SpendingByMood { get; set; } = new();
        public List<MoodTrendDto> MoodTrend { get; set; } = new();
        public EmotionalInsightsDto Insights { get; set; } = new();
    }

    public class SpendingByMoodDto
    {
        public string Mood { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransaction { get; set; }
    }

    public class MoodTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public string DominantMood { get; set; } = string.Empty;
        public decimal TotalSpending { get; set; }
        public Dictionary<string, decimal> MoodBreakdown { get; set; } = new();
    }

    public class EmotionalInsightsDto
    {
        public string HighestSpendingMood { get; set; } = string.Empty;
        public decimal HighestSpendingMoodAmount { get; set; }
        public string MostFrequentMood { get; set; } = string.Empty;
        public int MostFrequentMoodCount { get; set; }
        public decimal AverageEmotionalSpending { get; set; }
        public decimal PercentageWithMoodTracked { get; set; }
    }
}
