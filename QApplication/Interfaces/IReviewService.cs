using QApplication.Requests.EmployeeRequest;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IReviewService
{
    IEnumerable<ReviewResponseModel> GetAll(int pageList, int pageNumber);
    ReviewResponseModel GetById(int id);
    ReviewResponseModel Add(ReviewRequestModel request);
    ReviewResponseModel Update(int id, ReviewRequestModel request);
    bool Delete(int id);
}