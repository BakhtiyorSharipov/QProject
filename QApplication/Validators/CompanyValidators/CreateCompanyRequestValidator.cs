using FluentValidation;
using QApplication.Requests.CompanyRequest;

namespace QApplication.Validators;

public class CreateCompanyRequestValidator: AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        Include(new CompanyRequestValidator());
    }
}