using FluentValidation;
using QApplication.Requests.BlockedCustomerRequest;

namespace QApplication.Validators.BlockedCustomerValidators;

public class BlockedCustomerRequestValidator: AbstractValidator<BlockedCustomerRequestModel>
{
    public BlockedCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be greater than 0");

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("CompanyId must be greater than 0");
        
        RuleFor(x=>x.Reason)
            .MinimumLength(10).WithMessage("Blocked reason must be at least 10 characters")
            .MaximumLength(500).WithMessage("Blocked reason must be at most 500 characters");

        RuleFor(x => x.BannedUntil)
            .NotEmpty().WithMessage("Ban end date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Ban end date must be in the future.");
        

    }
}