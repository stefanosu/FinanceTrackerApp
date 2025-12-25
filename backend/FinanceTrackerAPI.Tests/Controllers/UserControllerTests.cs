using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using FinanceTrackerAPI.FinanceTracker.API;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<IUserService> _mockUserService;

        public UserControllerTests()
        {
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public async Task GetUserById_WithValidId_ReturnsUserDto()
        {
            // Arrange
            var expectedUser = new UserDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Role = "User"
            };

            _mockUserService
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(expectedUser);

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act
            var result = await controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(1, returnedUser.Id);
            Assert.Equal("John", returnedUser.FirstName);
            Assert.Equal("john@example.com", returnedUser.Email);
            _mockUserService.Verify(x => x.GetUserByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetUserById_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _mockUserService
                .Setup(x => x.GetUserByIdAsync(999))
                .ThrowsAsync(new NotFoundException("User", 999));

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetUserById(999));
            _mockUserService.Verify(x => x.GetUserByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUserDtos()
        {
            // Arrange
            var expectedUsers = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Role = "User"
                },
                new UserDto
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    Role = "Admin"
                }
            };

            _mockUserService
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(expectedUsers);

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act
            var result = await controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Collection(returnedUsers,
                user => Assert.Equal("John", user.FirstName),
                user => Assert.Equal("Jane", user.FirstName)
            );
            _mockUserService.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateUser_WithValidDto_ReturnsCreatedUserDto()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Password = "SecurePass123!",
                Role = "User"
            };

            var expectedUser = new UserDto
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Role = "User"
            };

            _mockUserService
                .Setup(x => x.CreateUserAsync(It.IsAny<CreateUserDto>()))
                .ReturnsAsync(expectedUser);

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act
            var result = await controller.CreateUser(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("Jane", returnedUser.FirstName);
            Assert.Equal("jane@example.com", returnedUser.Email);
            Assert.Equal(1, returnedUser.Id);
            _mockUserService.Verify(x => x.CreateUserAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task CreateUser_WithNullDto_ThrowsValidationException()
        {
            // Arrange
            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => controller.CreateUser(null!));
            _mockUserService.Verify(x => x.CreateUserAsync(It.IsAny<CreateUserDto>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUser_WithValidDto_ReturnsUpdatedUserDto()
        {
            // Arrange
            var userId = 1;
            var updateDto = new CreateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.updated@example.com",
                Password = "NewSecurePass123!",
                Role = "Admin"
            };

            var expectedUser = new UserDto
            {
                Id = userId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.updated@example.com",
                Role = "Admin"
            };

            _mockUserService
                .Setup(x => x.UpdateUserAsync(userId, It.IsAny<CreateUserDto>()))
                .ReturnsAsync(expectedUser);

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act
            var result = await controller.UpdateUser(userId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
            Assert.Equal("jane.updated@example.com", returnedUser.Email);
            Assert.Equal("Admin", returnedUser.Role);
            _mockUserService.Verify(x => x.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 999;
            var updateDto = new CreateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Password = "SecurePass123!",
                Role = "User"
            };

            _mockUserService
                .Setup(x => x.UpdateUserAsync(userId, It.IsAny<CreateUserDto>()))
                .ThrowsAsync(new NotFoundException("User", userId));

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.UpdateUser(userId, updateDto));
            _mockUserService.Verify(x => x.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithNullDto_ThrowsValidationException()
        {
            // Arrange
            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => controller.UpdateUser(1, null!));
            _mockUserService.Verify(x => x.UpdateUserAsync(It.IsAny<int>(), It.IsAny<CreateUserDto>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_WithValidId_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(true);

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act
            var result = await controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully.", okResult.Value);
            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(false);

            var controller = new UserController(_mockLogger.Object, _mockUserService.Object);

            // Act
            var result = await controller.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"User with ID {userId} not found.", notFoundResult.Value);
            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }
    }
}
