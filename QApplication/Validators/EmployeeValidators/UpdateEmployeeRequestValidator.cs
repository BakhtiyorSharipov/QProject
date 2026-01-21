using FluentValidation;
using QApplication.Requests.EmployeeRequest;

namespace QApplication.Validators;

public class UpdateEmployeeRequestValidator: AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("Service ID must be a positive number.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Firstname is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Firstname must be at most 50 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("First name can only contain letters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Lastname is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Lastname must be at most 50 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Last name can only contain letters");

        
        RuleFor(x=>x.Position)
            .NotEmpty().WithMessage("Position is required")
            .MinimumLength(2).WithMessage("Position must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Position must be at most 50 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Position contains invalid characters.");

        
        RuleFor(x=>x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("PhoneNumber is invalid");

        
    }
}