using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinanceTrackerAPI.FinanceTracker.API.Filters
{
    public class ExceptionHandlingFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionHandlingFilter> _logger;

        public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    _logger.LogWarning("Resource not found: {Message}", notFoundEx.Message);
                    context.Result = new NotFoundObjectResult(notFoundEx.Message);
                    break;

                case ValidationException validationEx:
                    _logger.LogWarning("Validation error: {Message}", validationEx.Message);
                    context.Result = new BadRequestObjectResult(validationEx.Message);
                    break;

                default:
                    _logger.LogError(exception, "An unexpected error occurred");
                    context.Result = new ObjectResult("An error occurred while processing your request.")
                    {
                        StatusCode = 500
                    };
                    break;
            }

            context.ExceptionHandled = true;
        }
    }
}
