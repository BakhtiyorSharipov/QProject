using FluentValidation;
using QApplication.Requests.CustomerRequest;

namespace QApplication.Validators.CustomerValidators;

public class UpdateCustomerRequestValidator: AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        Include(new CustomerRequestValidator());
    }
}