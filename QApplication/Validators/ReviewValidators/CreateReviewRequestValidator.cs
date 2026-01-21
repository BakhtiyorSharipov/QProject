using FluentValidation;
using QApplication.Requests.ReviewRequest;

namespace QApplication.Validators.ReviewValidators;

public class CreateReviewRequestValidator: AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        Include(new ReviewRequestValidator());
    }
}