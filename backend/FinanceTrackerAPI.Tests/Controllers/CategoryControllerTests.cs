using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
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
            var mockCategories = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Food", Description = "Food and dining expenses" },
                new CategoryDto { Id = 2, Name = "Transport", Description = "Transportation costs" }
            };

            _mockCategoryService.Setup(x => x.GetAllCategoriesAsync()).ReturnsAsync(mockCategories);

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategories = Assert.IsType<List<CategoryDto>>(okResult.Value);
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
            var createDto = new CreateCategoryDto { Name = "Entertainment", Description = "Entertainment expenses" };
            var createdCategory = new CategoryDto { Id = 3, Name = "Entertainment", Description = "Entertainment expenses" };

            _mockCategoryService.Setup(x => x.CreateCategoryAsync(It.IsAny<CreateCategoryDto>())).ReturnsAsync(createdCategory);

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal("Entertainment", returnedCategory.Name);
            Assert.Equal(3, returnedCategory.Id);
        }

        [Fact]
        public async Task CreateCategory_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = "Invalid", Description = "Invalid category" };
            _mockCategoryService.Setup(x => x.CreateCategoryAsync(It.IsAny<CreateCategoryDto>())).ThrowsAsync(new ValidationException("Invalid category data"));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<ValidationException>(() => _controller.CreateCategory(createDto));
        }

        [Fact]
        public async Task UpdateCategory_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            var updateDto = new UpdateCategoryDto { Name = "Updated Food", Description = "Updated food description" };
            var updatedCategory = new CategoryDto { Id = 1, Name = "Updated Food", Description = "Updated food description" };

            _mockCategoryService.Setup(x => x.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>())).ReturnsAsync(updatedCategory);

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateDto);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(categoryId, returnedCategory.Id);
            Assert.Equal("Updated Food", returnedCategory.Name);
        }

        [Fact]
        public async Task UpdateCategory_WhenNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = 999;
            var updateDto = new UpdateCategoryDto { Name = "Non-existent", Description = "This category doesn't exist" };
            _mockCategoryService.Setup(x => x.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>())).ThrowsAsync(new NotFoundException("Category", categoryId));

            // Act & Assert - Exception propagates to GlobalExceptionHandler middleware
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateCategory(categoryId, updateDto));
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
