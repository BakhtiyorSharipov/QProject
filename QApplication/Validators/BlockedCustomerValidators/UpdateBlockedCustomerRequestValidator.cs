using FluentValidation;
using QApplication.Requests.BlockedCustomerRequest;

namespace QApplication.Validators.BlockedCustomerValidators;

public class UpdateBlockedCustomerRequestValidator: AbstractValidator<UpdateBlockedCustomerRequest>
{
    public UpdateBlockedCustomerRequestValidator()
    {
        RuleFor(x => x.DoesBanForever)
            .NotNull().WithMessage("DoesBanForever is required.");
    }
}