using FluentValidation;
using QApplication.Requests.CustomerRequest;

namespace QApplication.Validators.CustomerValidators;

public class CreateCustomerRequestValidator: AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        Include(new CustomerRequestValidator());
    }
}