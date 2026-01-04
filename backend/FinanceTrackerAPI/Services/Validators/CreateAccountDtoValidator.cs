using FinanceTrackerAPI.Services.Dtos;

using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Account name is required")
                .Length(2, 100).WithMessage("Account name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s'-]+$").WithMessage("Account name can only contain letters, numbers, spaces, hyphens, and apostrophes");

            RuleFor(x => x.InitialBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Initial balance must be greater than or equal to 0")
                .LessThanOrEqualTo(999999999.99m).WithMessage("Initial balance exceeds maximum allowed value");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // Note: Id is required in DTO but not used by service (service sets id = 0)
            // Consider removing Id from CreateAccountDto in the future
        }
    }
}

