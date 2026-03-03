using FluentValidation;
using QBranchService.Application.Requests;

namespace QBranchService.Application.Validators.CompanyValidators;

public class UpdateCompanyValidator: AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyValidator()
    {
        RuleFor(s => s.CompanyName)
            .NotEmpty()
            .WithMessage("Company Name  is required.");
        
        RuleFor(s=>s.Address)
            .NotEmpty()
            .WithMessage("Address is required");

        RuleFor(s => s.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("Phone number is invalid. Use formats: +1234567890, (123) 456-7890, or 123-456-7890")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(s => s.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .MaximumLength(50).WithMessage("Email address cannot exceed 50 characters.");
    }
}