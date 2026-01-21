using FluentValidation;
using QApplication.Requests.AvailabilityScheduleRequest;
using QDomain.Enums;

namespace QApplication.Validators.AvailabilityScheduleValidators;

public class AvailabilityScheduleRequestValidator: AbstractValidator<AvailabilityScheduleRequestModel>
{
    public AvailabilityScheduleRequestValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("EmployeeId must be greater than 0");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters");

        RuleFor(x => x.RepeatSlot)
            .IsInEnum().WithMessage("Invalid repeat slot value.");

        RuleFor(x => x.RepeatDuration)
            .GreaterThan(0).WithMessage("Repeat duration must be positive.")
            .When(x => x.RepeatSlot != RepeatSlot.None && x.RepeatDuration.HasValue)
            .LessThanOrEqualTo(365).WithMessage("Repeat duration cannot exceed 365 days.")
            .When(x => x.RepeatSlot != RepeatSlot.None && x.RepeatDuration.HasValue);


        RuleFor(x => x.AvailableSlots)
            .NotEmpty().WithMessage("At least one available slot is required.")
            .Must(slots => slots.Count <= 50).WithMessage("Cannot have more than 50 slots per schedule.");

        RuleForEach(x => x.AvailableSlots)
            .ChildRules(slot =>
            {
                slot.RuleFor(s => s.From)
                    .NotEmpty().WithMessage("Slot start time is required.")
                    .LessThan(s => s.To).WithMessage("Start time must be before end time.")
                    .GreaterThanOrEqualTo(DateTimeOffset.UtcNow.Date)
                    .WithMessage("Slot cannot be in the past.");

                slot.RuleFor(s => s.To)
                    .NotEmpty().WithMessage("Slot end time is required.")
                    .GreaterThan(s => s.From)
                    .WithMessage("End time must be after start time.");

                slot.RuleFor(s => s)
                    .Must(s => (s.To - s.From).TotalMinutes >= 15)
                    .WithMessage("Minimum slot duration is 15 minutes.")
                    .Must(s => (s.To - s.From).TotalHours <= 48)
                    .WithMessage("Maximum slot duration is 48 hours.");
            });
        
    }
}