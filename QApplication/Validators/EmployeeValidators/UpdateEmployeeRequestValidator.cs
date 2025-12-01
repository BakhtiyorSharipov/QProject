using FluentValidation;
using QApplication.Requests.EmployeeRequest;

namespace QApplication.Validators;

public class UpdateEmployeeRequestValidator: AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        Include(new EmployeeRequestValidator());
        
    }
}