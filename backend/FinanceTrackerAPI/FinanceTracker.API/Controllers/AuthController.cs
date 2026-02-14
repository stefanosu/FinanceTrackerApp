using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.Services.Dto;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [AllowAnonymous] // Login endpoint must be public
        [EnableRateLimiting("auth")] // STRICT: 5 attempts per minute - prevents brute force
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

                // Cross-origin cookies require SameSite=None and Secure=true
                // This allows Vercel frontend to receive cookies from Render backend
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Required for SameSite=None
                    SameSite = SameSiteMode.None, // Required for cross-origin
                    Expires = DateTimeOffset.UtcNow.AddDays(1) // Access token expires in 1 day
                };

                Response.Cookies.Append("accessToken", response.AccessToken, cookieOptions);

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7) // Refresh token expires in 7 days
                };

                Response.Cookies.Append("refreshToken", response.RefreshToken, refreshCookieOptions);

                // Return tokens in response body as well (for client-side access if needed)
                // In production, consider omitting tokens from response if using HTTP-only cookies only
                return Ok(response);
            }
            catch (FinanceTrackerAPI.FinanceTracker.Domain.Exceptions.ValidationException ex)
            {
                // AuthController needs to return 401 Unauthorized for ValidationException
                // instead of 400 BadRequest (which GlobalExceptionHandler would return)
                _logger.LogWarning("Login failed for email: {Email}, Reason: {Message}", request.Email, ex.Message);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Authentication failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized
                });
            }
        }

        [HttpPost("logout")]
        [Authorize] // Must be authenticated to logout
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Logout()
        {
            // Clear authentication cookies
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1) // Expire immediately
            };

            Response.Cookies.Delete("accessToken", cookieOptions);
            Response.Cookies.Delete("refreshToken", cookieOptions);

            _logger.LogInformation("User logged out successfully");

            return Ok(new { message = "Logged out successfully" });
        }
    }
}

