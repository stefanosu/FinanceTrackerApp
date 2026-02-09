using Moq;
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
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockTransactionService.Object);
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
        public async Task GetAllTransactions_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange - Mock service to throw exception
            _mockTransactionService.Setup(x => x.GetAllTransactionsAsync()).ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<Exception>(() => _controller.GetAllTransactions());
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
        public async Task CreateTransaction_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var newTransaction = new Transaction { AccountId = 1, Amount = -50, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Invalid amount" };
            _mockTransactionService.Setup(x => x.CreateTransactionAsync(newTransaction)).ThrowsAsync(new ValidationException("Invalid transaction data"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<ValidationException>(() => _controller.CreateTransaction(newTransaction));
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
        public async Task UpdateTransaction_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var transactionId = 999;
            var updateTransaction = new Transaction { AccountId = 1, Amount = 200, Type = "Expense", dateOnly = DateOnly.FromDateTime(DateTime.UtcNow), CategoryId = 1, Notes = "Non-existent transaction" };
            _mockTransactionService.Setup(x => x.UpdateTransactionAsync(transactionId, updateTransaction)).ThrowsAsync(new NotFoundException("Transaction", transactionId));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateTransaction(transactionId, updateTransaction));
        }

        [Fact]
        public async Task DeleteTransaction_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var transactionId = 1;
            // Mock returns Task<bool> per the interface - ReturnsAsync(true) creates Task<bool>
            _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("deleted successfully", okResult.Value?.ToString());
        }

        [Fact]
        public async Task DeleteTransaction_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var transactionId = 999;
            _mockTransactionService.Setup(x => x.DeleteTransactionAsync(transactionId)).ThrowsAsync(new NotFoundException("Transaction", transactionId));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteTransaction(transactionId));
        }
    }
}
