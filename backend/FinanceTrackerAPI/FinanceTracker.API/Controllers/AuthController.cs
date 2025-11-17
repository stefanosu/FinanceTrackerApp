using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.Services.Dto;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest? request)
        {
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "Request body is required.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            try
            {
                var response = await _authService.LoginAsync(request.Email, request.Password);

                // Set HTTP-only cookies for secure token storage
                var isDevelopment = HttpContext.RequestServices
                    .GetRequiredService<IHostEnvironment>().IsDevelopment();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !isDevelopment, // Use HTTPS in production
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(1) // Access token expires in 1 day
                };

                Response.Cookies.Append("accessToken", response.AccessToken, cookieOptions);

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !isDevelopment,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(7) // Refresh token expires in 7 days
                };

                Response.Cookies.Append("refreshToken", response.RefreshToken, refreshCookieOptions);

                // Return tokens in response body as well (for client-side access if needed)
                // In production, consider omitting tokens from response if using HTTP-only cookies only
                return Ok(response);
            }
            catch (FinanceTrackerAPI.FinanceTracker.Domain.Exceptions.ValidationException ex)
            {
                _logger.LogWarning("Login failed for email: {Email}, Reason: {Message}", request.Email, ex.Message);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Authentication failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Email);
                return Problem(
                    title: "An error occurred during login",
                    detail: "Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}

