using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Interfaces;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockCategoryService.Object);
        }

        [Fact]
        public async Task GetAllCategories_WithMockedData_ReturnsOkResult()
        {
            // Arrange
            var mockCategories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = 1, Name = "Food", Description = "Food and dining expenses" },
                new ExpenseCategory { Id = 2, Name = "Transport", Description = "Transportation costs" }
            };

            _mockCategoryService.Setup(x => x.GetAllCategoriesAsync()).ReturnsAsync(mockCategories);

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategories = Assert.IsType<List<ExpenseCategory>>(okResult.Value);
            Assert.Equal(2, returnedCategories.Count);
            Assert.Equal("Food", returnedCategories[0].Name);
            Assert.Equal("Transport", returnedCategories[1].Name);
        }

        [Fact]
        public async Task GetAllCategories_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange - Mock service to throw exception
            _mockCategoryService.Setup(x => x.GetAllCategoriesAsync()).ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<Exception>(() => _controller.GetAllCategories());
        }

        [Fact]
        public async Task CreateCategory_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var newCategory = new ExpenseCategory { Name = "Entertainment", Description = "Entertainment expenses" };
            var createdCategory = new ExpenseCategory { Id = 3, Name = "Entertainment", Description = "Entertainment expenses" };

            _mockCategoryService.Setup(x => x.CreateCategoryAsync(newCategory)).ReturnsAsync(createdCategory);

            // Act
            var result = await _controller.CreateCategory(newCategory);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<ExpenseCategory>(okResult.Value);
            Assert.Equal("Entertainment", returnedCategory.Name);
            Assert.Equal(3, returnedCategory.Id);
        }

        [Fact]
        public async Task CreateCategory_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var newCategory = new ExpenseCategory { Name = "Invalid", Description = "Invalid category" };
            _mockCategoryService.Setup(x => x.CreateCategoryAsync(newCategory)).ThrowsAsync(new ValidationException("Invalid category data"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<ValidationException>(() => _controller.CreateCategory(newCategory));
        }

        [Fact]
        public async Task UpdateCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            var updateCategory = new ExpenseCategory { Name = "Updated Food", Description = "Updated food description" };
            var updatedCategory = new ExpenseCategory { Id = 1, Name = "Updated Food", Description = "Updated food description" };

            _mockCategoryService.Setup(x => x.UpdateCategoryAsync(categoryId, updateCategory)).ReturnsAsync(updatedCategory);

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateCategory);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<ExpenseCategory>(okResult.Value);
            Assert.Equal(categoryId, returnedCategory.Id);
            Assert.Equal("Updated Food", returnedCategory.Name);
        }

        [Fact]
        public async Task UpdateCategory_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = 999;
            var updateCategory = new ExpenseCategory { Name = "Non-existent", Description = "This category doesn't exist" };
            _mockCategoryService.Setup(x => x.UpdateCategoryAsync(categoryId, updateCategory)).ThrowsAsync(new NotFoundException("Category", categoryId));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateCategory(categoryId, updateCategory));
        }

        [Fact]
        public async Task DeleteCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            _mockCategoryService.Setup(x => x.DeleteCategoryAsync(categoryId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("deleted successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteCategory_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = 999;
            _mockCategoryService.Setup(x => x.DeleteCategoryAsync(categoryId)).ThrowsAsync(new NotFoundException("Category", categoryId));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteCategory(categoryId));
        }
    }
}
