using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FinanceTrackerAPI.FinanceTracker.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = HttpStatusCode.NotFound;
                    errorResponse.Message = exception.Message;
                    break;

                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = exception.Message;
                    break;

                case ArgumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = exception.Message;
                    break;

                default:
                    // Log the exception for debugging
                    _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An error occurred while processing your request.";
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}