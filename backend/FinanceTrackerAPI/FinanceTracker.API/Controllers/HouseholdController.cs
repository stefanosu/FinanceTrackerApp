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
    public class HouseholdController : ControllerBase
    {
        private readonly IHouseholdService _householdService;
        private readonly ILogger<HouseholdController> _logger;

        public HouseholdController(IHouseholdService householdService, ILogger<HouseholdController> logger)
        {
            _householdService = householdService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserHousehold()
        {
            var household = await _householdService.GetUserHouseholdAsync();
            if (household == null)
                return Ok(new { hasHousehold = false });

            return Ok(new { hasHousehold = true, household });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetHouseholdSummary()
        {
            var summary = await _householdService.GetHouseholdSummaryAsync();
            if (summary == null)
                return Ok(new { hasHousehold = false });

            return Ok(summary);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateHousehold([FromBody] CreateHouseholdDto dto)
        {
            var household = await _householdService.CreateHouseholdAsync(dto);
            return Ok(household);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHousehold(int id, [FromBody] UpdateHouseholdDto dto)
        {
            var household = await _householdService.UpdateHouseholdAsync(id, dto);
            return Ok(household);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHousehold(int id)
        {
            await _householdService.DeleteHouseholdAsync(id);
            return Ok(new { message = "Household deleted successfully" });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinHousehold([FromBody] JoinHouseholdDto dto)
        {
            var household = await _householdService.JoinHouseholdAsync(dto);
            return Ok(household);
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveHousehold()
        {
            await _householdService.LeaveHouseholdAsync();
            return Ok(new { message = "Left household successfully" });
        }

        [HttpPost("{id}/regenerate-code")]
        public async Task<IActionResult> RegenerateInviteCode(int id)
        {
            var newCode = await _householdService.RegenerateInviteCodeAsync(id);
            return Ok(new { inviteCode = newCode });
        }

        [HttpDelete("{householdId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int householdId, int userId)
        {
            await _householdService.RemoveMemberAsync(householdId, userId);
            return Ok(new { message = "Member removed successfully" });
        }
    }
}
