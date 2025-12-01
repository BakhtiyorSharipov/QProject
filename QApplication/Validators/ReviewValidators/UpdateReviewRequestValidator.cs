using FluentValidation;
using QApplication.Requests.ReviewRequest;

namespace QApplication.Validators.ReviewValidators;

public class UpdateReviewRequestValidator: AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewRequestValidator()
    {
        Include(new ReviewRequestValidator());
    }
}                                                                                                                                                                                                                                                      
