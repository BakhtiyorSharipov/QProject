using QApplication.Requests.EmployeeRequest;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IReviewService
{
    IEnumerable<ReviewResponseModel> GetAll(int pageList, int pageNumber);
    IEnumerable<ReviewResponseModel> GetAllReviewsByQueue(int queueId);
    ReviewResponseModel GetById(int id);
    ReviewResponseModel Add(ReviewRequestModel request);
}