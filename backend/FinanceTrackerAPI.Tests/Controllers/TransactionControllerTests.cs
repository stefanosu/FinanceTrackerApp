using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Interfaces;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ILogger<TransactionController>> _mockLogger;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockLogger = new Mock<ILogger<TransactionController>>();
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockLogger.Object, _mockTransactionService.Object);
        }

        [Fact]
        public async Task GetAllTransactions_WithMockedData_ReturnsOkResult()
        {
            // Arrange
            var mockTransactions = new List<Transaction>
            {
                new Transaction { Id = 1, AccountId = 1, Amount = 100, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Grocery shopping" },
                new Transaction { Id = 2, AccountId = 1, Amount = 50, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 2, Notes = "Gas station" }
            };

            _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ReturnsAsync(mockTransactions);

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTransactions = Assert.IsType<List<Transaction>>(okResult.Value);
            Assert.Equal(2, returnedTransactions.Count);
            Assert.Equal(100.00M, returnedTransactions[0].Amount);
            Assert.Equal(50.00M, returnedTransactions[1].Amount);
        }

        [Fact]
        public async Task GetAllTransactions_WhenExceptionOccurs_ReturnsProblemResult()
        {
            // Arrange - Mock service to throw exception
            _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.GetAllTransactions();

            // Assert
            Assert.NotNull(result);
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.NotNull(problemResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var newTransaction = new Transaction { AccountId = 1, Amount = 75, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 3, Notes = "Coffee shop" };
            var createdTransaction = new Transaction { Id = 3, AccountId = 1, Amount = 75, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 3, Notes = "Coffee shop" };

            _mockTransactionService.Setup(x => x.CreateTransactionAsync(newTransaction)).ReturnsAsync(createdTransaction);

            // Act
            var result = await _controller.CreateTransaction(newTransaction);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTransaction = Assert.IsType<Transaction>(okResult.Value);
            Assert.Equal(75.00M, returnedTransaction.Amount);
            Assert.Equal(3, returnedTransaction.Id);
        }

        [Fact]
        public async Task CreateTransaction_WhenExceptionOccurs_ReturnsProblemResult()
        {
            // Arrange
            var newTransaction = new Transaction { AccountId = 1, Amount = -50, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Invalid amount" };
            _mockTransactionService.Setup(x => x.CreateTransactionAsync(newTransaction)).ThrowsAsync(new ValidationException("Invalid transaction data"));

            // Act
            var result = await _controller.CreateTransaction(newTransaction);

            // Assert
            Assert.NotNull(result);
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.NotNull(problemResult.Value);
        }

        [Fact]
        public async Task UpdateTransaction_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var transactionId = 1;
            var updateTransaction = new Transaction { AccountId = 1, Amount = 125, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Updated grocery shopping" };
            var updatedTransaction = new Transaction { Id = 1, AccountId = 1, Amount = 125, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Updated grocery shopping" };

            _mockTransactionService.Setup(x => x.UpdateTransactionAsync(transactionId, updateTransaction)).ReturnsAsync(updatedTransaction);

            // Act
            var result = await _controller.UpdateTransaction(transactionId, updateTransaction);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTransaction = Assert.IsType<Transaction>(okResult.Value);
            Assert.Equal(transactionId, returnedTransaction.Id);
            Assert.Equal(125.00M, returnedTransaction.Amount);
        }

        [Fact]
        public async Task UpdateTransaction_WhenExceptionOccurs_ReturnsProblemResult()
        {
            // Arrange
            var transactionId = 999;
            var updateTransaction = new Transaction { AccountId = 1, Amount = 200, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Non-existent transaction" };
            _mockTransactionService.Setup(x => x.UpdateTransactionAsync(transactionId, updateTransaction)).ThrowsAsync(new NotFoundException("Transaction", transactionId));

            // Act
            var result = await _controller.UpdateTransaction(transactionId, updateTransaction);

            // Assert
            Assert.NotNull(result);
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.NotNull(problemResult.Value);
        }

        [Fact]
        public async Task DeleteTransaction_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var transactionId = 1;
            _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTransaction_WhenServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            var transactionId = 1;
            _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Failed to delete", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTransaction_WhenExceptionOccurs_ReturnsProblemResult()
        {
            // Arrange
            var transactionId = 999;
            _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId)).ThrowsAsync(new NotFoundException("Transaction", transactionId));

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            Assert.NotNull(result);
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            Assert.NotNull(problemResult.Value);
        }
    }
}
