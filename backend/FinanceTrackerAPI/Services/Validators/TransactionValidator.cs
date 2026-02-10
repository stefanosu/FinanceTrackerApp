using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    /// <summary>
    /// Validates Transaction entities before they're processed by the service layer.
    ///
    /// Key Validation Concepts:
    /// 1. Required fields - prevent null/empty values
    /// 2. Range validation - amounts must be positive, IDs must be valid
    /// 3. Business rules - transaction types must be valid enum values
    /// 4. Date validation - prevent future dates or unreasonably old dates
    /// </summary>
    public class TransactionValidator : AbstractValidator<Transaction>
    {
        // Define valid transaction types as a constant for maintainability
        private static readonly string[] ValidTransactionTypes = { "Income", "Expense", "Transfer" };

        public TransactionValidator()
        {
            // ID validation - when updating, ID must be positive
            // Note: For creates, ID is typically 0 or ignored
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Transaction ID must be a non-negative integer");

            // AccountId - must reference a valid account
            RuleFor(x => x.AccountId)
                .GreaterThan(0)
                .WithMessage("AccountId must be a positive integer referencing a valid account");

            // Amount validation
            // Why not just > 0? Some systems allow zero-value transactions for corrections
            RuleFor(x => x.Amount)
                .NotEqual(0)
                .WithMessage("Transaction amount cannot be zero")
                .Must(amount => amount >= -999999999.99m && amount <= 999999999.99m)
                .WithMessage("Transaction amount must be between -999,999,999.99 and 999,999,999.99");

            // Type must be a valid transaction type
            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Transaction type is required")
                .Must(type => ValidTransactionTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Transaction type must be one of: {string.Join(", ", ValidTransactionTypes)}");

            // Date validation - not in the future, not unreasonably old
            RuleFor(x => x.dateOnly)
                .NotEmpty()
                .WithMessage("Transaction date is required")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
                .WithMessage("Transaction date cannot be in the future")
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-100)))
                .WithMessage("Transaction date is unreasonably old");

            // CategoryId validation
            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("CategoryId must be a positive integer referencing a valid category");

            // Notes - optional but if provided, should be reasonable length
            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
