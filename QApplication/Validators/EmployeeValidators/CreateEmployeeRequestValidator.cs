using FluentValidation;
using QApplication.Requests.EmployeeRequest;

namespace QApplication.Validators;

public class CreateEmployeeRequestValidator: AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        Include(new EmployeeRequestValidator());
    }
}