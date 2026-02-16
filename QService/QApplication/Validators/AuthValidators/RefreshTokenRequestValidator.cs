using FluentValidation;
using QApplication.Requests;

namespace QApplication.Validators.AuthValidators;

public class RefreshTokenRequestValidator: AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(20).WithMessage("Invalid refresh token format.")
            .MaximumLength(500).WithMessage("Invalid refresh token format.");
    }
}