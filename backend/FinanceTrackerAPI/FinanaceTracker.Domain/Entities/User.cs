using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }

        public User() {}

        public User(int id, string firstName, string lastName, string email, string password, string role, DateTime createdAt, DateTime updatedAt, string token, string refreshToken)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}