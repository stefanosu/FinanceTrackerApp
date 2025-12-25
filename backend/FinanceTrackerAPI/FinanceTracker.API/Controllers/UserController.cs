using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

namespace FinanceTrackerAPI.FinanceTracker.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService; 

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
                throw new ValidationException("User cannot be null.");

            var createdUser = await _userService.CreateUserAsync(createUserDto);
            return Ok(createdUser);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null)
                throw new ValidationException("User cannot be null.");

            var updatedUser = await _userService.UpdateUserAsync(userId, createUserDto);
            if (updatedUser == null)
                throw new NotFoundException("User", userId);

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (deleted)
                {
                    return Ok("User deleted successfully.");
                }
                else
                {
                    return NotFound($"User with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user: {UserId}", id);
                return Problem(title: "DeleteUser failed", detail: "An unexpected error occurred while deleting the user.");
            }
        }
    }
}
