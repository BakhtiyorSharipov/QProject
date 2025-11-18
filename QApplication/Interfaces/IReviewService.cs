using QApplication.Requests.EmployeeRequest;
using QApplication.Requests.ReviewRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponseModel>> GetAllAsync(int pageList, int pageNumber);
    Task<IEnumerable<ReviewResponseModel>> GetAllReviewsByQueueAsync(int queueId);
    Task<IEnumerable<ReviewResponseModel>> GetAllReviewsByCompanyAsync(int companyId);
    Task<ReviewResponseModel> GetByIdAsync(int id);
    Task<ReviewResponseModel> AddAsync(ReviewRequestModel request);
}