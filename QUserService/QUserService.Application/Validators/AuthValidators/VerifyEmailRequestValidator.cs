using FluentValidation;
using QUserService.Application.Requests;

namespace QUserService.Application.Validators.AuthValidators;

public class VerifyEmailRequestValidator: AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Matches(@"^\d+$").WithMessage("Code must contain only digits.")
            .Length(6).WithMessage("Code must be 6 digit");
        
    }
}