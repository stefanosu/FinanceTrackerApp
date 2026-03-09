using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    /// <summary>
    /// Validates RecurringTransaction entities before they're processed by the service layer.
    /// </summary>
    public class RecurringTransactionValidator : AbstractValidator<RecurringTransaction>
    {
        private static readonly string[] ValidTransactionTypes = { "Income", "Expense" };
        private static readonly string[] ValidFrequencies = { "Daily", "Weekly", "Biweekly", "Monthly", "Quarterly", "Yearly" };

        public RecurringTransactionValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Recurring transaction ID must be a non-negative integer");

            RuleFor(x => x.AccountId)
                .GreaterThan(0)
                .WithMessage("AccountId must be a positive integer referencing a valid account");

            RuleFor(x => x.Amount)
                .NotEqual(0)
                .WithMessage("Amount cannot be zero")
                .Must(amount => amount >= -999999999.99m && amount <= 999999999.99m)
                .WithMessage("Amount must be between -999,999,999.99 and 999,999,999.99");

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Transaction type is required")
                .Must(type => ValidTransactionTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Transaction type must be one of: {string.Join(", ", ValidTransactionTypes)}");

            RuleFor(x => x.Frequency)
                .NotEmpty()
                .WithMessage("Frequency is required")
                .Must(freq => ValidFrequencies.Contains(freq, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Frequency must be one of: {string.Join(", ", ValidFrequencies)}");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(200)
                .WithMessage("Description cannot exceed 200 characters");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("CategoryId must be a positive integer referencing a valid category");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .Must((transaction, endDate) => !endDate.HasValue || endDate.Value >= transaction.StartDate)
                .WithMessage("End date must be on or after start date")
                .When(x => x.EndDate.HasValue);
        }
    }
}
