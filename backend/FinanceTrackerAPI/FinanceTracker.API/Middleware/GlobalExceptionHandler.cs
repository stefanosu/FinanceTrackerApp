using System.Net;
using System.Text.Json;

using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinanceTrackerAPI.FinanceTracker.API.Middleware
{
    /// <summary>
    /// Global exception handler middleware.
    ///
    /// SECURITY PRINCIPLE: Never expose internal details to clients.
    /// - Log detailed info server-side for debugging
    /// - Return generic messages to clients
    /// - Include correlation ID for support to trace issues
    /// - Follow RFC 7807 Problem Details standard
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
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
            // Generate correlation ID for tracing
            var correlationId = context.TraceIdentifier;

            var response = context.Response;
            response.ContentType = "application/problem+json";

            // Ensure CORS headers on error responses - browser blocks cross-origin reads without them
            // Must match the same origins allowed in Program.cs CORS policy
            var origin = context.Request.Headers.Origin.FirstOrDefault();
            if (!string.IsNullOrEmpty(origin) && IsAllowedOrigin(origin))
            {
                response.Headers.Append("Access-Control-Allow-Origin", origin);
                response.Headers.Append("Access-Control-Allow-Credentials", "true");
            }

            // Determine status code and user-facing message
            var (statusCode, title, userMessage) = ClassifyException(exception);

            response.StatusCode = statusCode;

            // Log detailed info server-side (NEVER send to client)
            LogException(exception, correlationId, statusCode);

            // Build RFC 7807 Problem Details response
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = userMessage,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = correlationId,
                    ["timestamp"] = DateTime.UtcNow
                }
            };

            // In development, include more details for debugging
            // NEVER do this in production!
            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["debug"] = new
                {
                    exceptionType = exception.GetType().Name,
                    message = exception.Message,
                    stackTrace = exception.StackTrace?.Split('\n').Take(5) // First 5 lines only
                };
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            await response.WriteAsJsonAsync(problemDetails, options);
        }

        /// <summary>
        /// Classifies exceptions into HTTP status codes and user-safe messages.
        ///
        /// KEY SECURITY PATTERN:
        /// - Internal details go to logs
        /// - Generic messages go to clients
        /// - Same error types get same generic message (prevents enumeration attacks)
        /// </summary>
        private static (int StatusCode, string Title, string Message) ClassifyException(Exception exception)
        {
            return exception switch
            {
                // 404 - Resource not found
                // Generic message prevents enumeration (attacker can't tell if resource exists)
                NotFoundException => (
                    StatusCodes.Status404NotFound,
                    "Resource Not Found",
                    "The requested resource was not found."
                ),

                // 400 - Validation error
                // Show validation errors but sanitize sensitive field names
                ValidationException validationEx => (
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    SanitizeValidationMessage(validationEx.Message)
                ),

                // 400 - Bad argument
                ArgumentException => (
                    StatusCodes.Status400BadRequest,
                    "Invalid Request",
                    "The request contains invalid data."
                ),

                // 401 - Unauthorized
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "You are not authorized to access this resource."
                ),

                // 409 - Conflict (e.g., duplicate entry)
                InvalidOperationException when exception.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) => (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "The operation could not be completed due to a conflict."
                ),

                // 500 - Everything else
                // NEVER expose internal error details
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later."
                )
            };
        }

        /// <summary>
        /// Sanitizes validation messages to remove sensitive information.
        ///
        /// Examples of what to sanitize:
        /// - Email addresses: "Email 'john@example.com' is invalid" → "Email format is invalid"
        /// - IDs: "User with ID 12345 not found" → "The specified resource was not found"
        /// </summary>
        private static string SanitizeValidationMessage(string message)
        {
            // Remove email addresses from messages
            var sanitized = System.Text.RegularExpressions.Regex.Replace(
                message,
                @"'[^']*@[^']*'",
                "'[email]'"
            );

            // Remove specific IDs from messages
            sanitized = System.Text.RegularExpressions.Regex.Replace(
                sanitized,
                @"ID \d+",
                "ID [redacted]"
            );

            return sanitized;
        }

        /// <summary>
        /// Logs exception details server-side for debugging.
        /// Includes correlation ID for matching with client reports.
        /// </summary>
        private void LogException(Exception exception, string correlationId, int statusCode)
        {
            // Use structured logging for better searchability
            var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

            _logger.Log(
                logLevel,
                exception,
                "Exception occurred. CorrelationId: {CorrelationId}, StatusCode: {StatusCode}, Type: {ExceptionType}, Message: {Message}",
                correlationId,
                statusCode,
                exception.GetType().Name,
                exception.Message
            );
        }

        /// <summary>
        /// Checks if the origin is allowed for CORS.
        /// Must match the same logic as Program.cs CORS policy.
        /// </summary>
        private static bool IsAllowedOrigin(string origin)
        {
            try
            {
                var uri = new Uri(origin);
                // Allow localhost for development
                if (uri.Host == "localhost") return true;
                // Allow main Vercel domain
                if (uri.Host == "finance-tracker-app-ivory.vercel.app") return true;
                // Allow all Vercel preview deployments
                if (uri.Host.EndsWith(".vercel.app")) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
