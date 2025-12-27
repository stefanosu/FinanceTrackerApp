using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FinanceTrackerAPI.FinanceTracker.API.Controllers;
using FinanceTrackerAPI.Services.Interfaces;
using FinanceTrackerAPI.Services.Dto;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;

namespace FinanceTrackerAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockLogger.Object, _mockAuthService.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "john@example.com",
                Password = "password123"
            };

            var expectedResponse = new LoginResponse
            {
                AccessToken = "mock-access-token",
                RefreshToken = "mock-refresh-token"
            };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginRequest.Email, loginRequest.Password))
                .ReturnsAsync(expectedResponse);

            // Setup HttpContext for cookie operations
            var hostEnvironment = new Mock<IHostEnvironment>();
            hostEnvironment.Setup(x => x.EnvironmentName).Returns("Development");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IHostEnvironment>(hostEnvironment.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsAssignableFrom<ObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var response = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal("mock-access-token", response.AccessToken);
            Assert.Equal("mock-refresh-token", response.RefreshToken);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "john@example.com",
                Password = "wrongpassword"
            };

            _mockAuthService
                .Setup(x => x.LoginAsync(loginRequest.Email, loginRequest.Password))
                .ThrowsAsync(new ValidationException("Invalid email or password"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.NotNull(result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Login(null);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

