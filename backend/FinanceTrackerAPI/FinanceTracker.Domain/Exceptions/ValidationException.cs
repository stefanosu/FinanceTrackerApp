using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}