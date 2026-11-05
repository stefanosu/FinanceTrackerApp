using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("api")]
    public class EmotionalAnalyticsController : ControllerBase
    {
        private readonly IEmotionalAnalyticsService _emotionalAnalyticsService;
        private readonly ILogger<EmotionalAnalyticsController> _logger;

        public EmotionalAnalyticsController(
            IEmotionalAnalyticsService emotionalAnalyticsService,
            ILogger<EmotionalAnalyticsController> logger)
        {
            _emotionalAnalyticsService = emotionalAnalyticsService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetEmotionalAnalyticsSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var summary = await _emotionalAnalyticsService.GetEmotionalAnalyticsSummaryAsync(startDate, endDate);
            return Ok(summary);
        }

        [HttpGet("spending-by-mood")]
        public async Task<IActionResult> GetSpendingByMood(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var spending = await _emotionalAnalyticsService.GetSpendingByMoodAsync(startDate, endDate);
            return Ok(spending);
        }

        [HttpGet("mood-trend")]
        public async Task<IActionResult> GetMoodTrend([FromQuery] int months = 6)
        {
            if (months < 1 || months > 24)
                months = 6;

            _logger.LogInformation("Getting mood trend for {Months} months", months);
            var trend = await _emotionalAnalyticsService.GetMoodTrendAsync(months);
            return Ok(trend);
        }

        [HttpGet("insights")]
        public async Task<IActionResult> GetEmotionalInsights(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var insights = await _emotionalAnalyticsService.GetEmotionalInsightsAsync(startDate, endDate);
            return Ok(insights);
        }
    }
}
