using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Interfaces;
using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountController(_mockLogger.Object, _mockAccountService.Object);
        }

        [Fact]
        public async Task GetAllAccounts_WithMockedData_ReturnsOkResult()
        {
            // Arrange
            var mockAccounts = new List<AccountDto>
            {
                new AccountDto { Id = 1, Name = "Test Account 1", AccountType = "Savings", Description = "Test account 1", Balance = 1000, CreatedAt = DateTime.UtcNow },
                new AccountDto { Id = 2, Name = "Test Account 2", AccountType = "Checking", Description = "Test account 2", Balance = 500, CreatedAt = DateTime.UtcNow }
            };

            _mockAccountService.Setup(x => x.GetAllAccountsAsync()).ReturnsAsync(mockAccounts);

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccounts = Assert.IsType<List<AccountDto>>(okResult.Value);
            Assert.Equal(2, returnedAccounts.Count);
            Assert.Equal("Test Account 1", returnedAccounts[0].Name);
            Assert.Equal("Test Account 2", returnedAccounts[1].Name);
        }

        [Fact]
        public async Task GetAllAccounts_WhenExceptionOccurs_ReturnsProblemResult()
        {
            // Arrange - Mock service to throw exception
            _mockAccountService.Setup(x => x.GetAllAccountsAsync()).ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            Assert.NotNull(result);
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            // The controller returns Problem() which creates a ProblemDetails object
            Assert.NotNull(problemResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var accountId = 1;
            var updateAccountDto = new UpdateAccountDto { Name = "Updated Account", AccountType = "Savings" };
            var updatedAccountDto = new AccountDto { Id = accountId, Name = "Updated Account", AccountType = "Savings", Description = "Updated account", Balance = 1500, CreatedAt = DateTime.UtcNow };

            _mockAccountService.Setup(x => x.UpdateAccountAsync(accountId, updateAccountDto)).ReturnsAsync(updatedAccountDto);

            // Act
            var result = await _controller.UpdateAccount(accountId, updateAccountDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccount = Assert.IsType<AccountDto>(okResult.Value);
            Assert.Equal(accountId, returnedAccount.Id);
            Assert.Equal("Updated Account", returnedAccount.Name);
        }
    }
}