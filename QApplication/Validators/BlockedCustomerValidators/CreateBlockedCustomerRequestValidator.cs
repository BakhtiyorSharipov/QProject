using FluentValidation;
using QApplication.Requests.BlockedCustomerRequest;

namespace QApplication.Validators.BlockedCustomerValidators;

public class CreateBlockedCustomerRequestValidator: AbstractValidator<CreateBlockedCustomerRequest>
{
    public CreateBlockedCustomerRequestValidator()
    {
        Include(new BlockedCustomerRequestValidator());
    }
}