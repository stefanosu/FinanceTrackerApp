using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.FinanceTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly FinanceTrackerDbContext _dbContext;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            // Create in-memory database
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _dbContext = new FinanceTrackerDbContext(options);
            _mockLogger = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_mockLogger.Object, _dbContext);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var testAccounts = new List<Account>
            {
                new Account { id = 1, Name = "Test Account 1", Email = "test1@example.com", AccountType = "Savings" },
                new Account { id = 2, Name = "Test Account 2", Email = "test2@example.com", AccountType = "Checking" }
            };

            _dbContext.Accounts.AddRange(testAccounts);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllAccounts_WithMockedData_ReturnsOkResult()
        {
            // Act - Call the method under test
            var result = await _controller.GetAllAccounts();

            // Assert - Verify the expected behavior
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccounts = Assert.IsType<List<Account>>(okResult.Value);
            Assert.Equal(2, returnedAccounts.Count);
            Assert.Equal("Test Account 1", returnedAccounts[0].Name);
            Assert.Equal("Test Account 2", returnedAccounts[1].Name);
        }

        [Fact]
        public async Task GetAllAccounts_WhenExceptionOccurs_ReturnsProblemResult()
        {
            // Arrange - Clear the database to simulate an error
            _dbContext.Accounts.RemoveRange(_dbContext.Accounts.ToList());
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccounts = Assert.IsType<List<Account>>(okResult.Value);
            Assert.Empty(returnedAccounts);
        }

        [Fact]
        public async Task UpdateAccount_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var accountId = 1;
            var updateAccount = new Account { id = accountId, Name = "Updated Account", Email = "updated@example.com", AccountType = "Savings" };

            // Act
            var result = await _controller.UpdateAccount(accountId, updateAccount);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(accountId, returnedAccount.id);
            Assert.Equal("Updated Account", returnedAccount.Name);
        }
    }
}