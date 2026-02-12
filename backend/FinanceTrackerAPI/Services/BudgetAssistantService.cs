using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services.Dto.AI;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class BudgetAssistantService : IBudgetAssistantService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BudgetAssistantService> _logger;

        public BudgetAssistantService(
            FinanceTrackerDbContext context,
            ICurrentUserService currentUserService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<BudgetAssistantService> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<ChatResponseDto> GetChatResponseAsync(string message)
        {
            var apiKey = _configuration["Claude:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Claude API key not configured");
                return new ChatResponseDto
                {
                    Message = "I'm sorry, but the AI assistant is not configured. Please set up your Claude API key to enable this feature.",
                    Timestamp = DateTime.UtcNow
                };
            }

            try
            {
                var financialContext = await GetFinancialContextAsync();
                var systemPrompt = BuildSystemPrompt(financialContext);
                var response = await CallClaudeApiAsync(systemPrompt, message, apiKey);
                return new ChatResponseDto
                {
                    Message = response,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in budget assistant: {Message}", ex.Message);
                return new ChatResponseDto
                {
                    Message = "I'm having trouble connecting right now. Please try again later.",
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        public async Task<FinancialContextDto> GetFinancialContextAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
                return new FinancialContextDto();

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            // Get total balance
            var totalBalance = await _context.Accounts.SumAsync(a => a.Balance);

            // Get current month expenses
            var currentMonthExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= startOfMonth)
                .SumAsync(e => e.Amount);

            // Get last month expenses for trend comparison
            var lastMonthExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= startOfLastMonth && e.Date < startOfMonth)
                .SumAsync(e => e.Amount);

            // Get current month income
            var startDateOnly = DateOnly.FromDateTime(startOfMonth);
            var currentMonthIncome = await _context.Transactions
                .Where(t => t.Date >= startDateOnly && (t.Type == "Income" || t.Type == "Deposit"))
                .SumAsync(t => t.Amount);

            // Get top spending categories
            var categorySpending = await _context.Expenses
                .Where(e => e.UserId == userId.Value && e.Date >= startOfMonth)
                .GroupBy(e => e.Category)
                .Select(g => new CategorySpending
                {
                    Category = g.Key,
                    Amount = g.Sum(e => e.Amount),
                    Percentage = 0
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToListAsync();

            // Calculate percentages
            if (currentMonthExpenses > 0)
            {
                foreach (var category in categorySpending)
                {
                    category.Percentage = Math.Round(category.Amount / currentMonthExpenses * 100, 1);
                }
            }

            // Get recent expenses
            var recentExpenses = await _context.Expenses
                .Where(e => e.UserId == userId.Value)
                .OrderByDescending(e => e.Date)
                .Take(5)
                .Select(e => new RecentExpense
                {
                    Name = e.Name,
                    Amount = e.Amount,
                    Category = e.Category,
                    Date = e.Date
                })
                .ToListAsync();

            // Calculate savings rate
            var savingsRate = currentMonthIncome > 0
                ? Math.Round((currentMonthIncome - currentMonthExpenses) / currentMonthIncome * 100, 1)
                : 0;

            // Determine spending trend
            var spendingTrend = "stable";
            if (lastMonthExpenses > 0)
            {
                var changePercent = (currentMonthExpenses - lastMonthExpenses) / lastMonthExpenses * 100;
                if (changePercent > 10) spendingTrend = "increasing";
                else if (changePercent < -10) spendingTrend = "decreasing";
            }

            return new FinancialContextDto
            {
                TotalBalance = totalBalance,
                MonthlyIncome = currentMonthIncome,
                MonthlyExpenses = currentMonthExpenses,
                SavingsRate = savingsRate,
                TopCategories = categorySpending,
                RecentExpenses = recentExpenses,
                SpendingTrend = spendingTrend
            };
        }

        private static string BuildSystemPrompt(FinancialContextDto context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are a helpful financial assistant for a personal finance tracking app.");
            sb.AppendLine("You help users understand their spending habits and provide personalized budgeting advice.");
            sb.AppendLine("Be concise, friendly, and actionable in your responses.");
            sb.AppendLine("Focus on practical tips the user can implement immediately.");
            sb.AppendLine();
            sb.AppendLine("Here is the user's current financial context:");
            sb.AppendLine($"- Total Balance: ${context.TotalBalance:N2}");
            sb.AppendLine($"- This Month's Income: ${context.MonthlyIncome:N2}");
            sb.AppendLine($"- This Month's Expenses: ${context.MonthlyExpenses:N2}");
            sb.AppendLine($"- Savings Rate: {context.SavingsRate}%");
            sb.AppendLine($"- Spending Trend: {context.SpendingTrend}");
            sb.AppendLine();

            if (context.TopCategories.Count > 0)
            {
                sb.AppendLine("Top spending categories this month:");
                foreach (var cat in context.TopCategories)
                {
                    sb.AppendLine($"  - {cat.Category}: ${cat.Amount:N2} ({cat.Percentage}%)");
                }
                sb.AppendLine();
            }

            if (context.RecentExpenses.Count > 0)
            {
                sb.AppendLine("Recent expenses:");
                foreach (var exp in context.RecentExpenses)
                {
                    sb.AppendLine($"  - {exp.Name}: ${exp.Amount:N2} ({exp.Category}) on {exp.Date:MMM dd}");
                }
            }

            return sb.ToString();
        }

        private async Task<string> CallClaudeApiAsync(string systemPrompt, string userMessage, string apiKey)
        {
            _logger.LogInformation("Calling Claude API with message: {Message}", userMessage);

            var requestBody = new
            {
                model = "claude-3-haiku-20240307",
                max_tokens = 1024,
                system = systemPrompt,
                messages = new[]
                {
                    new { role = "user", content = userMessage }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var response = await _httpClient.PostAsync("https://api.anthropic.com/v1/messages", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Claude API response status: {StatusCode}", response.StatusCode);
            _logger.LogInformation("Claude API response: {Response}", responseContent.Length > 500 ? responseContent.Substring(0, 500) : responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Claude API error: {StatusCode} - {Content}", response.StatusCode, responseContent);
                throw new HttpRequestException($"Claude API returned {response.StatusCode}: {responseContent}");
            }

            using var doc = JsonDocument.Parse(responseContent);
            var textContent = doc.RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString();

            _logger.LogInformation("Parsed response text: {Text}", textContent?.Substring(0, Math.Min(100, textContent?.Length ?? 0)));

            return textContent ?? "I couldn't generate a response.";
        }
    }
}
