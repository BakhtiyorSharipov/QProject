using FluentValidation;
using QBranchService.Application.UseCases.CompanyServices.Commands.CreateService;

namespace QBranchService.Application.Validators.CompanyServiceValidators;

public class CreateCompanyServiceValidator: AbstractValidator<CreateServiceCommand>
{
    public CreateCompanyServiceValidator()
    {
        RuleFor(s => s.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.")
            .GreaterThan(0).WithMessage("CompanyId ,ust be greater than 0.");
            
        
        RuleFor(s => s.ServiceName)
            .NotEmpty().WithMessage("Service name is required.")
            .MaximumLength(30).WithMessage("Service name cannot exceed 30 characters.");

        RuleFor(s => s.ServiceDescription)
            .NotEmpty().WithMessage("ServiceDescription is required.")
            .MaximumLength(50).WithMessage("Service description cannot exceed 50 characters.");

        

    }
}