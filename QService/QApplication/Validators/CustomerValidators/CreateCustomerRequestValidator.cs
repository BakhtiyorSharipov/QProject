using FluentValidation;
using QApplication.Requests.CustomerRequest;
using QApplication.UseCases.Customers.Commands.CreateCustomer;

namespace QApplication.Validators.CustomerValidators;

public class CreateCustomerRequestValidator: AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("Firstname is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Firstname must be at most 50 characters.");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Lastname is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Lastname must be at most 50 characters.");
        
        RuleFor(x=>x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^[\+]?[0-9\s\-\(\)]{8,20}$")
            .WithMessage("PhoneNumber is invalid");
    }
    
    
}