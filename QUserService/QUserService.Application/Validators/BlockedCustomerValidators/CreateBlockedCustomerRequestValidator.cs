using FluentValidation;
using QUserService.Application.UseCases.BlockedCustomers.Commands.CreateBlockedCustomer;

namespace QUserService.Application.Validators.BlockedCustomerValidators;

public class CreateBlockedCustomerRequestValidator: AbstractValidator<CreateBlockedCustomerCommand>
{
    public CreateBlockedCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be greater than 0");
        
        
        RuleFor(x=>x.Reason)
            .MinimumLength(10).WithMessage("Blocked reason must be at least 10 characters")
            .MaximumLength(500).WithMessage("Blocked reason must be at most 500 characters");

        RuleFor(x => x.BannedUntil)
            .NotEmpty().WithMessage("Ban and date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Ban and date must be in the future.");
        

    }
}