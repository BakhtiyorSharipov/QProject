using FluentValidation;
using QApplication.Requests.ServiceRequest;
using QApplication.UseCases.Services.Commands.CreateService;

namespace QApplication.Validators.ServiceValidator;

public class CreateServiceRequestValidator: AbstractValidator<CreateServiceCommand>
{
    public CreateServiceRequestValidator()
    {
        RuleFor(x => x.ServiceName)
            .NotEmpty().WithMessage("Service name is required")
            .MinimumLength(2).WithMessage("Service name  must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Service name  must be at most 50 characters.");

        RuleFor(x => x.ServiceDescription)
            .NotEmpty().WithMessage("Service description is required")
            .MinimumLength(2).WithMessage("Service description  must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Service description  must be at most 100 characters.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required")
            .GreaterThan(0).WithMessage("CompanyId must be greater than 0");
    }
}