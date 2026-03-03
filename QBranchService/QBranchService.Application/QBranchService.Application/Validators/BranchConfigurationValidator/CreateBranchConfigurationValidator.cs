using FluentValidation;
using QBranchService.Application.UseCases.BranchConfigurations.Commands.CreateBranchConfiguration;

namespace QBranchService.Application.Validators.BranchConfigurationValidator;

public class CreateBranchConfigurationValidator: AbstractValidator<CreateBranchConfigurationCommand>
{
    public CreateBranchConfigurationValidator()
    {
        RuleFor(s => s.BranchId)
            .NotEmpty().WithMessage("BranchId is required.")
            .GreaterThan(0).WithMessage("BranchId ,must be greater than 0.");

        RuleFor(s => s.MaxTicketsPerDay)
            .NotEmpty().WithMessage("MaxTicketsPerDay is required")
            .GreaterThan(0).WithMessage("BranchId ,must be greater than 0.");

        RuleFor(s => s.OpenTime)
            .NotEmpty().WithMessage("OpenTime is required.");

        RuleFor(s => s.CloseTime)
            .NotEmpty().WithMessage("CloseTime is required.")
            .GreaterThan(s => s.OpenTime)
            .WithMessage("CloseTime must be after OpenTime");

        RuleFor(s => s.BreakStartTime)
            .NotEmpty().When(s => s.BreakStartTime.HasValue)
            .WithMessage("BreakStartTime is required when BreakEndTime is provided.");

        RuleFor(s => s.BreakEndTime)
            .NotEmpty().When(s => s.BreakEndTime.HasValue)
            .WithMessage("BreakEndTime is required when BreakStartTime is provided.");

        When(s => s.BreakStartTime.HasValue && s.BreakEndTime.HasValue, () =>
        {
            RuleFor(s => s.BreakStartTime)
                .GreaterThanOrEqualTo(s => s.OpenTime)
                .WithMessage("BreakStartTime must be after or equal to OpenTime.")
                .LessThan(s => s.CloseTime)
                .WithMessage("BreakStartTime must be before CloseTime.");

            RuleFor(s => s.BreakEndTime)
                .GreaterThan(s => s.BreakStartTime)
                .WithMessage("BreakEndTime must be after BreakStartTime.")
                .LessThanOrEqualTo(s => s.CloseTime)
                .WithMessage("BreakEndTime must be before or equal to CloseTime.");
        });
        
        
    }
}