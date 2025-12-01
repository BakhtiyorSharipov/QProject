using FluentValidation;
using QApplication.Requests.ServiceRequest;

namespace QApplication.Validators.ServiceValidator;

public class UpdateServiceRequestValidator: AbstractValidator<UpdateServiceRequest>
{
    public UpdateServiceRequestValidator()
    {
        Include(new ServiceRequestValidator());
    }
}