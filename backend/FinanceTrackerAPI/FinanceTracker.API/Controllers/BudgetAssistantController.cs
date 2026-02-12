using FinanceTrackerAPI.Services.Dto.AI;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("ai")]
    public class BudgetAssistantController : ControllerBase
    {
        private readonly IBudgetAssistantService _budgetAssistantService;
        private readonly ILogger<BudgetAssistantController> _logger;

        public BudgetAssistantController(
            IBudgetAssistantService budgetAssistantService,
            ILogger<BudgetAssistantController> logger)
        {
            _budgetAssistantService = budgetAssistantService;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            _logger.LogInformation("Chat endpoint called with message: {Message}", request.Message);
            var response = await _budgetAssistantService.GetChatResponseAsync(request.Message);
            _logger.LogInformation("Chat response: {Response}", response.Message?.Substring(0, Math.Min(100, response.Message?.Length ?? 0)));
            return Ok(response);
        }

        [HttpGet("context")]
        public async Task<IActionResult> GetContext()
        {
            var context = await _budgetAssistantService.GetFinancialContextAsync();
            return Ok(context);
        }
    }
}
