using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpPatch] 
        public IActionResult UpdateUser(User user) 
        {
            if(user.Id == user.Id) 
            {
                //update a user with updated user attrs and save that user obj to db
                var updatedUser = User(user);
            }
        }
    }
}

public class FinanceTrackerDbContext : DbContext
{
    public FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
}