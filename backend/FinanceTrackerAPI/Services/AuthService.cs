using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dto;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTrackerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            FinanceTrackerDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException("Email and password are required.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
                throw new ValidationException("Invalid email or password.");
            }

            // Verify password hash
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                _logger.LogWarning("Invalid password attempt for email: {Email}", email);
                throw new ValidationException("Invalid email or password.");
            }

            // Generate JWT tokens
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Update user tokens in database
            user.Token = accessToken;
            user.RefreshToken = refreshToken;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User logged in successfully: {Email}", email);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "FinanceTrackerAPI";
            var audience = jwtSettings["Audience"] ?? "FinanceTrackerApp";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

