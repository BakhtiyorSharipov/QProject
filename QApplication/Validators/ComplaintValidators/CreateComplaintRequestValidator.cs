using FluentValidation;
using QApplication.Requests.ComplaintRequest;

namespace QApplication.Validators.ComplaintValidators;

public class CreateComplaintRequestValidator: AbstractValidator<CreateComplaintRequest>
{
    public CreateComplaintRequestValidator()
    {
        Include(new ComplaintRequestValidator());
    }
}