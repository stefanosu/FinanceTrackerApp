using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}