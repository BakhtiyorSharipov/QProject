using FluentValidation;
using QApplication.Requests;
using QApplication.UseCases.Auth.Commands.RegisterCustomer;

namespace QApplication.Validators.AuthValidators;

public class RegisterCustomerRequestValidator: AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Firstname is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Firstname must be at most 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Lastname is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Lastname must be at most 50 characters.");
        
        RuleFor(x=>x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("PhoneNumber is invalid");

        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("EmailAddress is required")
            .EmailAddress().WithMessage("EmailAddress is not valid")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password must be at most 100 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\!\?\*\.\$#@%^&+=]").WithMessage("Password must contain at least one special character (!?*.$#@%^&+=).");


    }
}