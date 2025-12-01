using FluentValidation;
using QApplication.Requests.QueueRequest;

namespace QApplication.Validators.QueueValidators;

public class CreateQueueRequestValidator: AbstractValidator<CreateQueueRequest>
{
    public CreateQueueRequestValidator()
    {
        Include(new QueueRequestModelValidator());
    }
}