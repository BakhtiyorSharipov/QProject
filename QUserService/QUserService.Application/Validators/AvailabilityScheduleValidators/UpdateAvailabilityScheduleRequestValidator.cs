using FluentValidation;
using QUserService.Application.Requests.AvailabilityScheduleRequest;

namespace QUserService.Application.Validators.AvailabilityScheduleValidators;

public class UpdateAvailabilityScheduleRequestValidator: AbstractValidator<UpdateAvailabilityScheduleRequest>
{
    public UpdateAvailabilityScheduleRequestValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        
        RuleFor(x => x.RepeatSlot)
            .IsInEnum().WithMessage("Invalid repeat slot value.");
        
        RuleFor(x => x.RepeatDuration)
            .GreaterThanOrEqualTo(0).When(x => x.RepeatDuration.HasValue)
            .WithMessage("Repeat duration must be positive.");
        
        RuleFor(x => x.AvailableSlots)
            .NotEmpty().WithMessage("At least one available slot is required.")
            .When(x => x.AvailableSlots != null);
    }
}