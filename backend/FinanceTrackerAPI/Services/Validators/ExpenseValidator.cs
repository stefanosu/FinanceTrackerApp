using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    /// <summary>
    /// Validates Expense entities.
    ///
    /// Security Considerations:
    /// 1. String length limits - prevent DoS via oversized payloads
    /// 2. Amount limits - prevent integer overflow attacks
    /// 3. UserId validation - will be overridden by authenticated user in service layer
    /// </summary>
    public class ExpenseValidator : AbstractValidator<Expense>
    {
        // Valid payment methods - could be moved to database/configuration
        private static readonly string[] ValidPaymentMethods =
        {
            "Cash", "Credit Card", "Debit Card", "Bank Transfer",
            "Check", "Digital Wallet", "Other"
        };

        public ExpenseValidator()
        {
            // Name validation
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Expense name is required")
                .MaximumLength(100)
                .WithMessage("Expense name cannot exceed 100 characters")
                .MinimumLength(2)
                .WithMessage("Expense name must be at least 2 characters");

            // Description - optional but limited
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters");

            // Amount must be positive (expenses are outflows)
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Expense amount must be greater than zero")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("Expense amount cannot exceed 999,999,999.99");

            // Date validation
            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Expense date is required")
                .Must(date => date <= DateTime.UtcNow.AddDays(1))
                .WithMessage("Expense date cannot be in the future")
                .Must(date => date >= DateTime.UtcNow.AddYears(-100))
                .WithMessage("Expense date is unreasonably old");

            // Category validation
            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required")
                .MaximumLength(50)
                .WithMessage("Category cannot exceed 50 characters");

            // SubCategory validation
            RuleFor(x => x.SubCategory)
                .MaximumLength(50)
                .WithMessage("SubCategory cannot exceed 50 characters");

            // Payment method validation
            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Payment method is required")
                .Must(method => ValidPaymentMethods.Contains(method, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Payment method must be one of: {string.Join(", ", ValidPaymentMethods)}");

            // Notes - optional with length limit
            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters");

            // UserId - must be positive
            // SECURITY NOTE: In controller, this should be overridden with authenticated user's ID
            // Never trust client-provided UserId!
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be a positive integer");
        }
    }
}
