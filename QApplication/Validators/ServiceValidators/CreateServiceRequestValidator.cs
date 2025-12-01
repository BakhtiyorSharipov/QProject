using FluentValidation;
using QApplication.Requests.ServiceRequest;

namespace QApplication.Validators.ServiceValidator;

public class CreateServiceRequestValidator: AbstractValidator<CreateServiceRequest>
{
    public CreateServiceRequestValidator()
    {
        Include(new ServiceRequestValidator());
    }
}