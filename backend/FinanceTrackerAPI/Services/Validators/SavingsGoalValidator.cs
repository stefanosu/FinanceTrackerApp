using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    /// <summary>
    /// Validates SavingsGoal entities before they're processed by the service layer.
    /// </summary>
    public class SavingsGoalValidator : AbstractValidator<SavingsGoal>
    {
        private static readonly string[] ValidCategories = { "Emergency Fund", "Vacation", "Home", "Car", "Education", "Retirement", "Wedding", "Electronics", "Other" };

        public SavingsGoalValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Savings goal ID must be a non-negative integer");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0)
                .WithMessage("Target amount must be greater than zero")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("Target amount cannot exceed 999,999,999.99");

            RuleFor(x => x.CurrentAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Current amount cannot be negative")
                .LessThanOrEqualTo(x => x.TargetAmount * 2)
                .WithMessage("Current amount seems unreasonably high");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required")
                .Must(cat => ValidCategories.Contains(cat, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Category must be one of: {string.Join(", ", ValidCategories)}");

            RuleFor(x => x.TargetDate)
                .Must(date => !date.HasValue || date.Value >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Target date cannot be in the past")
                .When(x => x.TargetDate.HasValue);
        }
    }
}
