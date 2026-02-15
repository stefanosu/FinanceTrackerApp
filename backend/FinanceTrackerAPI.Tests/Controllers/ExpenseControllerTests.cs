using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
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
            var mockExpenses = new List<ExpenseDto>
            {
                new ExpenseDto {
                    Id = 1,
                    Name = "Groceries",
                    Amount = 50.00M,
                    Description = "Grocery shopping",
                    Date = DateTime.Now,
                    Category = "Food",
                    SubCategory = "Groceries",
                    PaymentMethod = "Credit Card",
                    Notes = "Weekly groceries"
                },
                new ExpenseDto {
                    Id = 2,
                    Name = "Gas",
                    Amount = 30.00M,
                    Description = "Gas station",
                    Date = DateTime.Now,
                    Category = "Transportation",
                    SubCategory = "Fuel",
                    PaymentMethod = "Cash",
                    Notes = "Car fuel"
                }
            };

            _mockExpenseService.Setup(x => x.GetAllExpensesAsync())
                .ReturnsAsync(mockExpenses);

            // Act - Call the method you're testing
            var result = await _controller.GetAllExpenses();

            // Assert - Verify the results are what you expect
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpenses = Assert.IsType<List<ExpenseDto>>(okResult.Value);
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
            var mockExpense = new ExpenseDto
            {
                Id = expenseId,
                Name = "Groceries",
                Amount = 50.00M,
                Description = "Grocery shopping",
                Date = DateTime.Now,
                Category = "Food",
                SubCategory = "Groceries",
                PaymentMethod = "Credit Card",
                Notes = "Weekly groceries"
            };

            _mockExpenseService.Setup(x => x.GetExpenseByIdAsync(expenseId))
                .ReturnsAsync(mockExpense);

            // Act
            var result = await _controller.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpense = Assert.IsType<ExpenseDto>(okResult.Value);
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

        [Fact]
        public async Task CreateExpense_WithValidDto_ReturnsOkResult()
        {
            // Arrange
            var createDto = new CreateExpenseDto
            {
                Name = "Coffee",
                Amount = 5.00M,
                Description = "Morning coffee",
                Date = DateTime.Now,
                Category = "Food & Dining",
                SubCategory = "Coffee",
                PaymentMethod = "Cash",
                Notes = "Daily coffee"
            };

            var createdExpense = new ExpenseDto
            {
                Id = 1,
                Name = createDto.Name,
                Amount = createDto.Amount,
                Description = createDto.Description,
                Date = createDto.Date,
                Category = createDto.Category,
                SubCategory = createDto.SubCategory,
                PaymentMethod = createDto.PaymentMethod,
                Notes = createDto.Notes
            };

            _mockExpenseService.Setup(x => x.CreateExpenseAsync(It.IsAny<CreateExpenseDto>()))
                .ReturnsAsync(createdExpense);

            // Act
            var result = await _controller.CreateExpense(createDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpense = Assert.IsType<ExpenseDto>(okResult.Value);
            Assert.Equal(1, returnedExpense.Id);
            Assert.Equal("Coffee", returnedExpense.Name);

            // Verify service was called
            _mockExpenseService.Verify(x => x.CreateExpenseAsync(It.IsAny<CreateExpenseDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateExpense_WithValidDto_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            var updateDto = new UpdateExpenseDto
            {
                Name = "Updated Groceries",
                Amount = 75.00M
            };

            var updatedExpense = new ExpenseDto
            {
                Id = expenseId,
                Name = "Updated Groceries",
                Amount = 75.00M,
                Description = "Grocery shopping",
                Date = DateTime.Now,
                Category = "Food",
                SubCategory = "Groceries",
                PaymentMethod = "Credit Card",
                Notes = "Weekly groceries"
            };

            _mockExpenseService.Setup(x => x.UpdateExpenseAsync(expenseId, It.IsAny<UpdateExpenseDto>()))
                .ReturnsAsync(updatedExpense);

            // Act
            var result = await _controller.UpdateExpense(expenseId, updateDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpense = Assert.IsType<ExpenseDto>(okResult.Value);
            Assert.Equal(expenseId, returnedExpense.Id);
            Assert.Equal("Updated Groceries", returnedExpense.Name);
            Assert.Equal(75.00M, returnedExpense.Amount);

            // Verify service was called
            _mockExpenseService.Verify(x => x.UpdateExpenseAsync(expenseId, It.IsAny<UpdateExpenseDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteExpense_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            _mockExpenseService.Setup(x => x.DeleteExpenseAsync(expenseId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Expense deleted successfully.", okResult.Value);

            // Verify service was called
            _mockExpenseService.Verify(x => x.DeleteExpenseAsync(expenseId), Times.Once);
        }

        [Fact]
        public async Task DeleteExpense_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            var expenseId = 999;
            _mockExpenseService.Setup(x => x.DeleteExpenseAsync(expenseId))
                .ThrowsAsync(new NotFoundException("Expense", expenseId));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteExpense(expenseId));

            // Verify service was called
            _mockExpenseService.Verify(x => x.DeleteExpenseAsync(expenseId), Times.Once);
        }
    }
}
