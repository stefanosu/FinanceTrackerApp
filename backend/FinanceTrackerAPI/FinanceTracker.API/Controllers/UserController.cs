using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly FinanceTrackerDbContext _context;

        public UserController(ILogger<UserController> logger, FinanceTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("User", userId);

            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                throw new ValidationException("User cannot be null.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user == null)
                throw new ValidationException("User cannot be null.");

            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                throw new NotFoundException("User", user.Id);

            // Update the existing user with new attributes
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.Role = user.Role;
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.Token = user.Token;
            existingUser.RefreshToken = user.RefreshToken;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                throw new NotFoundException("User", id);

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }
    }
}