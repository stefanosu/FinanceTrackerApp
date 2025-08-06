using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
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
    }
}