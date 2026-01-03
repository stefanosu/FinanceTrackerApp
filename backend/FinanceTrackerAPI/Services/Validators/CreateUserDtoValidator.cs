using FluentValidation;
using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
                .Matches(@"^(?=.*[a-zA-Z])[a-zA-Z\s'-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters")
                .Matches(@"^(?=.*[a-zA-Z])[a-zA-Z\s'-]+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
                .Matches(@"^(?=.*[a-z])").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"^(?=.*[A-Z])").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"^(?=.*\d)").WithMessage("Password must contain at least one number");

            RuleFor(x => x.Role)
                .MaximumLength(50).WithMessage("Role must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Role));
        }
    }
}

