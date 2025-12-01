using FluentValidation;
using QApplication.Requests.CustomerRequest;

namespace QApplication.Validators.CustomerValidators;

public class CustomerRequestValidator: AbstractValidator<CustomerRequestModel>
{
    public CustomerRequestValidator()
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
    }
    
    
}