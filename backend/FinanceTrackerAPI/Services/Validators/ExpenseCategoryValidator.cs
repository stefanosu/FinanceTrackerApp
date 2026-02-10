using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    /// <summary>
    /// Validates ExpenseCategory entities.
    ///
    /// Categories are typically managed by admins, but validation prevents:
    /// 1. Empty/null category names
    /// 2. Excessively long strings (DoS prevention)
    /// 3. Duplicate category creation (handled at service/DB level)
    /// </summary>
    public class ExpenseCategoryValidator : AbstractValidator<ExpenseCategory>
    {
        public ExpenseCategoryValidator()
        {
            // Name validation
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Category name is required")
                .MaximumLength(50)
                .WithMessage("Category name cannot exceed 50 characters")
                .MinimumLength(2)
                .WithMessage("Category name must be at least 2 characters")
                // Prevent special characters that could cause issues
                .Matches(@"^[a-zA-Z0-9\s\-_&]+$")
                .WithMessage("Category name can only contain letters, numbers, spaces, hyphens, underscores, and ampersands");

            // Description validation
            RuleFor(x => x.Description)
                .MaximumLength(200)
                .WithMessage("Description cannot exceed 200 characters");
        }
    }
}
