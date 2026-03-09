using FinanceTrackerAPI.Services.Dtos;
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
    public class SavingsGoalController : ControllerBase
    {
        private readonly ISavingsGoalService _savingsGoalService;

        public SavingsGoalController(ISavingsGoalService savingsGoalService)
        {
            _savingsGoalService = savingsGoalService;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<SavingsGoalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSavingsGoals()
        {
            var goals = await _savingsGoalService.GetAllSavingsGoalsAsync();
            return Ok(goals);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSavingsGoalById(int id)
        {
            var goal = await _savingsGoalService.GetSavingsGoalByIdAsync(id);
            return Ok(goal);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSavingsGoal([FromBody] CreateSavingsGoalDto dto)
        {
            var created = await _savingsGoalService.CreateSavingsGoalAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSavingsGoal(int id, [FromBody] UpdateSavingsGoalDto dto)
        {
            var updated = await _savingsGoalService.UpdateSavingsGoalAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSavingsGoal(int id)
        {
            await _savingsGoalService.DeleteSavingsGoalAsync(id);
            return Ok("Savings goal deleted successfully.");
        }

        [HttpPost("{id}/contribute")]
        [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ContributeToGoal(int id, [FromBody] ContributeToGoalDto dto)
        {
            var updated = await _savingsGoalService.ContributeToGoalAsync(id, dto);
            return Ok(updated);
        }
    }
}
