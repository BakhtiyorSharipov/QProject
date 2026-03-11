using FluentValidation;
using QBranchService.Application.Requests;

namespace QBranchService.Application.Validators.BranchValidators;

public class UpdateBranchValidator: AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchValidator()
    {
        RuleFor(s => s.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.")
            .GreaterThan(0).WithMessage("CompanyId ,ust be greater than 0.");
            
        
        RuleFor(s => s.BranchName)
            .NotEmpty().WithMessage("Branch name is required.");

        RuleFor(s => s.City)
            .NotEmpty().WithMessage("City is required");

        RuleFor(s => s.Address)
            .NotEmpty().WithMessage("Address is required.");
        
        RuleFor(s => s.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("Phone number is invalid. Use formats: +1234567890, (123) 456-7890, or 123-456-7890")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(s => s.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .MaximumLength(50).WithMessage("Email address cannot exceed 100 characters.");
    }
}