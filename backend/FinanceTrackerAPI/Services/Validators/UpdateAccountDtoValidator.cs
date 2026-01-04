using FinanceTrackerAPI.Services.Dtos;

using FluentValidation;

namespace FinanceTrackerAPI.Services.Validators
{
    public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
    {
        public UpdateAccountDtoValidator()
        {
            // Name is optional, but if provided, must be valid
            RuleFor(x => x.Name)
                .Length(2, 100).WithMessage("Account name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s'-]+$").WithMessage("Account name can only contain letters, numbers, spaces, hyphens, and apostrophes")
                .When(x => !string.IsNullOrEmpty(x.Name));

            // Email is optional, but if provided, must be valid
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // AccountType is optional, but if provided, must be valid
            RuleFor(x => x.AccountType)
                .Length(2, 50).WithMessage("Account type must be between 2 and 50 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Account type can only contain letters and spaces")
                .When(x => !string.IsNullOrEmpty(x.AccountType));

            // Description is optional, but if provided, must be valid
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}

