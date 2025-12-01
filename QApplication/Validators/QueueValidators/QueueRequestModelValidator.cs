using FluentValidation;
using QApplication.Requests.QueueRequest;

namespace QApplication.Validators.QueueValidators;

public class QueueRequestModelValidator: AbstractValidator<QueueRequestModel>
{
    public QueueRequestModelValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be greater than 0");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("EmployeeId must be greater than 0");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("ServiceId must be greater than 0");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required")
            .GreaterThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Start time must be in the present or future.")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddDays(30))
            .WithMessage("Start time cannot be more than 30 days in advance.");
    }
}