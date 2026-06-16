using FluentValidation;
using QUserService.Application.Requests;

namespace QUserService.Application.Validators.AuthValidators;

public class ForgotPasswordRequestValidator: AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

    }
}