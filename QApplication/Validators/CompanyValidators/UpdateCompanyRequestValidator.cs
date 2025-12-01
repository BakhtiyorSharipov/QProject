using FluentValidation;
using QApplication.Requests.CompanyRequest;

namespace QApplication.Validators;

public class UpdateCompanyRequestValidator: AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        Include(new CompanyRequestValidator());
    }
}