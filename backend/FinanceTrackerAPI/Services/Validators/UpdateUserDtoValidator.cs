using FluentValidation;
using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            // FirstName is optional, but if provided, must be valid
            RuleFor(x => x.FirstName)
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
                .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            // LastName is optional, but if provided, must be valid
            RuleFor(x => x.LastName)
                .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters")
                .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            // Email is optional, but if provided, must be valid
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // Password is optional, but if provided, must meet complexity requirements
            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
                .Matches(@"^(?=.*[a-z])").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"^(?=.*[A-Z])").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"^(?=.*\d)").WithMessage("Password must contain at least one number")
                .When(x => !string.IsNullOrEmpty(x.Password));

            // Role is optional, but if provided, must be valid
            RuleFor(x => x.Role)
                .MaximumLength(50).WithMessage("Role must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Role));
        }
    }
}

