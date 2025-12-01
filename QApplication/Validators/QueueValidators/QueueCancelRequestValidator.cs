using FluentValidation;
using QApplication.Requests.QueueRequest;

namespace QApplication.Validators.QueueValidators;

public class QueueCancelRequestValidator: AbstractValidator<QueueCancelRequest>
{
    public QueueCancelRequestValidator()
    {
        RuleFor(x => x.QueueId)
            .GreaterThan(0).WithMessage("QueueId must be greater than 0.");
        
        RuleFor(x=>x.CancelReason)
            .NotEmpty().WithMessage("Cancel reason is required when cancelling a queue.")
            .MinimumLength(20).WithMessage("Cancel reason must be at least 20 characters")
            .MaximumLength(500).WithMessage("Cancel reason must be at most 500 characters");
    }
}