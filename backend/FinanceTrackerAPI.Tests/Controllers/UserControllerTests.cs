using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FinanceTrackerAPI.FinanceTracker.API;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly DbContextOptions<FinanceTrackerDbContext> _options;

        public UserControllerTests()
        {
            _mockLogger = new Mock<ILogger<UserController>>();
            _options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            using var context = new FinanceTrackerDbContext(_options);
            var controller = new UserController(_mockLogger.Object, context);

            var testUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123",
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Token = "test-token",
                RefreshToken = "test-refresh-token"
            };

            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetUserByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(1, returnedUser.Id);
            Assert.Equal("John", returnedUser.FirstName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            using var context = new FinanceTrackerDbContext(_options);
            var controller = new UserController(_mockLogger.Object, context);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetUserByIdAsync(999));
        }

        [Fact]
        public async Task CreateUser_WithValidUser_ReturnsCreatedUser()
        {
            // Arrange
            using var context = new FinanceTrackerDbContext(_options);
            var controller = new UserController(_mockLogger.Object, context);

            var newUser = new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Password = "password123",
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Token = "test-token",
                RefreshToken = "test-refresh-token"
            };

            // Act
            var result = await controller.CreateUser(newUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("Jane", returnedUser.FirstName);
            Assert.Equal("jane@example.com", returnedUser.Email);
        }

        [Fact]
        public async Task CreateUser_WithNullUser_ThrowsValidationException()
        {
            // Arrange
            using var context = new FinanceTrackerDbContext(_options);
            var controller = new UserController(_mockLogger.Object, context);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => controller.CreateUser(null));
        }
    }
}