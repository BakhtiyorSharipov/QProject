using FluentValidation;
using QApplication.Requests.AvailabilityScheduleRequest;

namespace QApplication.Validators.AvailabilityScheduleValidators;

public class CreateAvailabilityScheduleRequestValidator: AbstractValidator<CreateAvailabilityScheduleRequest>
{
    public CreateAvailabilityScheduleRequestValidator()
    {
        Include(new CreateAvailabilityScheduleRequestValidator());
    }
}