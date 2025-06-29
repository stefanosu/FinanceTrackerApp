using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
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

        [HttpGet]
        public IActionResult GetUser(User user) 
        {
            return Ok("Users");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (user == null)
                return BadRequest("User cannot be null.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user) 
        {
            if (user == null) 
            {
                return BadRequest("User cannot be null.");
            }

            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

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
    }
}