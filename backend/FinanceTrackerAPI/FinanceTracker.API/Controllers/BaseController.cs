using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<IActionResult> HandleServiceResult<T>(Func<Task<T>> serviceCall)
        {
            try
            {
                var result = await serviceCall();
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        protected async Task<IActionResult> HandleServiceResult(Func<Task> serviceCall)
        {
            try
            {
                await serviceCall();
                return Ok();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        protected bool ValidateId(int id)
        {
            if (id <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
