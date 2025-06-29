using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerAPI.FinanceTracker.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetUser(User user) 
        {
            return Ok("Users");
        }

        [HttpPost]
        public IActionResult CreateUser(User user) 
        {
            if (user == null)  
            {
                return BadRequest("User cannot be null.");
            }

            // Example: create a new user instance (copying properties)
            var newUser = new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Token = user.Token,
                RefreshToken = user.RefreshToken
            };

            // You would typically save newUser to the database here

            return Ok(newUser);
        }
    }
}