using FluentValidation;
using QApplication.Requests;
using QApplication.UseCases.Auth.Queries.Login;

namespace QApplication.Validators.AuthValidators;

public class LoginRequestValidator: AbstractValidator<LoginQuery>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\!\?\*\.\$#@%^&+=]").WithMessage("Password must contain at least one special character (!?*.$#@%^&+=).");

    }
}