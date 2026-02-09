using Xunit;
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
        private readonly Mock<IExpenseService> _mockExpenseService;
        private readonly ExpenseController _controller;

        public ExpenseControllerTests()
        {
            _mockExpenseService = new Mock<IExpenseService>();
            _controller = new ExpenseController(_mockExpenseService.Object);
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
            var result = await _controller.GetAllExpenses();

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

            // Act
            var result = await _controller.GetExpenseById(expenseId);

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
        public async Task GetExpenseById_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            var expenseId = 999;
            _mockExpenseService.Setup(x => x.GetExpenseByIdAsync(expenseId))
                .ThrowsAsync(new NotFoundException($"Expense with ID {expenseId} not found"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetExpenseById(expenseId));

            // Verify service was called
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(expenseId), Times.Once);
        }

        [Fact]
        public async Task GetExpenseById_WhenServiceThrowsException_ThrowsException()
        {
            // Arrange
            var expenseId = 1;
            _mockExpenseService.Setup(x => x.GetExpenseByIdAsync(expenseId))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<Exception>(() => _controller.GetExpenseById(expenseId));

            // Verify service was called
            _mockExpenseService.Verify(x => x.GetExpenseByIdAsync(expenseId), Times.Once);
        }
    }
}