using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using backend.Services.Interfaces;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class ExpenseControllerTests
    {
        private readonly Mock<ILogger<ExpenseController>> _mockLogger;
        private readonly Mock<IExpenseService> _mockExpenseService;

        public ExpenseControllerTests()
        {
            _mockLogger = new Mock<ILogger<ExpenseController>>();
            _mockExpenseService = new Mock<IExpenseService>();
        }

        [Fact]
        public async Task GetAllExpenses_WithMockedData_ReturnsOkResult()
        {
            // Arrange - Set up your test data and conditions
            var mockExpenses = new List<Expense>
            {
                new Expense {
                    Id = 1,
                    Name = "Groceries",
                    Amount = 50.00M,
                    Description = "Grocery shopping",
                    Date = DateTime.Now,
                    Category = "Food",
                    SubCategory = "Groceries",
                    PaymentMethod = "Credit Card",
                    Notes = "Weekly groceries",
                    UserId = 1
                },
                new Expense {
                    Id = 2,
                    Name = "Gas",
                    Amount = 30.00M,
                    Description = "Gas station",
                    Date = DateTime.Now,
                    Category = "Transportation",
                    SubCategory = "Fuel",
                    PaymentMethod = "Cash",
                    Notes = "Car fuel",
                    UserId = 1
                }
            };

            _mockExpenseService.Setup(x => x.GetAllExpensesAsync())
                .ReturnsAsync(mockExpenses);

            // Act - Call the method you're testing
            var controller = new ExpenseController(_mockLogger.Object, _mockExpenseService.Object);
            var result = await controller.GetAllExpenses();

            // Assert - Verify the results are what you expect
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpenses = Assert.IsType<List<Expense>>(okResult.Value);
            Assert.Equal(2, returnedExpenses.Count);
            Assert.Equal("Groceries", returnedExpenses[0].Name);
            Assert.Equal("Gas", returnedExpenses[1].Name);

            // Verify the service was called
            _mockExpenseService.Verify(x => x.GetAllExpensesAsync(), Times.Once);
        }

        // GetExpenseById Tests
        [Fact]
        public async Task GetExpenseById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            var mockExpense = new Expense
            {
                Id = expenseId,
                Name = "Groceries",
                Amount = 50.00M,
                Description = "Grocery shopping",
                Date = DateTime.Now,
                Category = "Food",
                SubCategory = "Groceries",
                PaymentMethod = "Credit Card",
                Notes = "Weekly groceries",
                UserId = 1
            };

            _mockExpenseService.Setup(x => x.GetExpenseByIdAsync(expenseId))
                .ReturnsAsync(mockExpense);

            var controller = new ExpenseController(_mockLogger.Object, _mockExpenseService.Object);

            // Act
            var result = await controller.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpense = Assert.IsType<Expense>(okResult.Value);
            Assert.Equal(expenseId, returnedExpense.Id);
            Assert.Equal("Groceries", returnedExpense.Name);

            // Verify service was called with correct parameter
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(expenseId), Times.Once);
        }

        [Fact]
        public async Task GetExpenseById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var expenseId = 999;
            _mockExpenseService.Setup(x => x.GetExpenseByIdAsync(expenseId))
                .ThrowsAsync(new NotFoundException($"Expense with ID {expenseId} not found"));

            var controller = new ExpenseController(_mockLogger.Object, _mockExpenseService.Object);

            // Act
            var result = await controller.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains($"Expense with ID {expenseId} not found", notFoundResult.Value.ToString());

            // Verify service was called
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(expenseId), Times.Once);
        }

        [Fact]
        public async Task GetExpenseById_WithZeroId_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = 0;
            var controller = new ExpenseController(_mockLogger.Object, _mockExpenseService.Object);

            // Act
            var result = await controller.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Invalid expense ID", badRequestResult.Value.ToString());

            // Verify service was NOT called with invalid ID
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetExpenseById_WithNegativeId_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = -1;
            var controller = new ExpenseController(_mockLogger.Object, _mockExpenseService.Object);

            // Act
            var result = await controller.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Invalid expense ID", badRequestResult.Value.ToString());

            // Verify service was NOT called with invalid ID
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetExpenseById_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var expenseId = 1;
            _mockExpenseService.Setup(x => x.GetExpenseByIdAsync(expenseId))
                .ThrowsAsync(new Exception("Database connection failed"));

            var controller = new ExpenseController(_mockLogger.Object, _mockExpenseService.Object);

            // Act
            var result = await controller.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("An error occurred", statusCodeResult.Value.ToString());

            // Verify service was called
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(expenseId), Times.Once);
        }
    }
}