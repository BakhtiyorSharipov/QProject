using FluentValidation;
using QApplication.Requests.CompanyRequest;
using QApplication.UseCase.Companies.Commands;

namespace QApplication.Validators;

public class CompanyRequestValidator: AbstractValidator<CreateCompanyCommand>
{
    public CompanyRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("CompanyName is required.")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Company name must be at most 100 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(2).WithMessage("Address must be at least 2 characters")
            .MaximumLength(100).WithMessage("Address must be at most 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("PhoneNumber is invalid.");

        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("EmailAddress is required.")
            .EmailAddress().WithMessage("EmailAddress is not valid")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");
        
    }
}