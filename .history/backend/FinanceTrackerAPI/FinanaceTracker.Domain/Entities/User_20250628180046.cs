
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
    }

    public User(int id, string firstName, string lastName, string email, string password, string role, DateTime createdAt, DateTime updatedAt, string token, string refreshToken)
    {
        Id = id;
        FirstName = FirstName;
        LastName = LastName;
        Email = Email;
        Password = Password;
        Role = Role;
        CreatedAt = CreatedAt;
        UpdatedAt = UpdatedAt;
        Token = Token;
        RefreshToken = RefreshToken;
    }
}   