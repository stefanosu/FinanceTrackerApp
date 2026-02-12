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
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAnalyticsSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var summary = await _analyticsService.GetAnalyticsSummaryAsync(startDate, endDate);
            return Ok(summary);
        }

        [HttpGet("spending-by-category")]
        public async Task<IActionResult> GetSpendingByCategory(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var spending = await _analyticsService.GetSpendingByCategoryAsync(startDate, endDate);
            return Ok(spending);
        }

        [HttpGet("financial-summary")]
        public async Task<IActionResult> GetFinancialSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var summary = await _analyticsService.GetFinancialSummaryAsync(startDate, endDate);
            return Ok(summary);
        }

        [HttpGet("monthly-trend")]
        public async Task<IActionResult> GetMonthlyTrend([FromQuery] int months = 6)
        {
            try
            {
                if (months < 1 || months > 24)
                    months = 6;

                _logger.LogInformation("Getting monthly trend for {Months} months", months);
                var trend = await _analyticsService.GetMonthlyTrendAsync(months);
                _logger.LogInformation("Monthly trend returned {Count} items", trend.Count);
                return Ok(trend);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly trend: {Message}", ex.Message);
                throw; // Let GlobalExceptionHandler handle it
            }
        }

        [HttpGet("month-comparison")]
        public async Task<IActionResult> GetMonthComparison()
        {
            var comparison = await _analyticsService.GetMonthComparisonAsync();
            return Ok(comparison);
        }
    }
}
