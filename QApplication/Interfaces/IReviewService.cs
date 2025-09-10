using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IReviewService: IBaseService<ReviewEntity, ReviewResponseModel, ReviewRequestModel>
{
    
}