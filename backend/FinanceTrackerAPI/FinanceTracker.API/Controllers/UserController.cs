using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTrackerAPI.FinanceTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints by default
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            ICurrentUserService currentUserService)
        {
            _logger = logger;
            _userService = userService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Gets the currently authenticated user's profile
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            return Ok(user);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("create")]
        [AllowAnonymous] // Registration endpoint must be public
        [EnableRateLimiting("auth")] // STRICT: 5 attempts per minute - prevents spam registration
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var createdUser = await _userService.CreateUserAsync(createUserDto);
            return Ok(createdUser);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("User deleted successfully.");
        }
    }
}
