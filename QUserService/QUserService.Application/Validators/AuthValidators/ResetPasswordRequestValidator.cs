using FluentValidation;
using QUserService.Application.UseCases.Auth.Commands.ResetPassword;

namespace QUserService.Application.Validators.AuthValidators;

public class ResetPasswordRequestValidator: AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Matches(@"^\d+$").WithMessage("Code must contain only digits.")
            .Length(6).WithMessage("Code must be 6 digit");

        
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password must be at most 100 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\!\?\*\.\$#@%^&+=]").WithMessage("Password must contain at least one special character (!?*.$#@%^&+=).");

        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password must be at most 100 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\!\?\*\.\$#@%^&+=]").WithMessage("Password must contain at least one special character (!?*.$#@%^&+=).");

    }
}