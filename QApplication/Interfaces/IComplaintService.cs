using QApplication.Requests.ComplaintRequest;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.Interfaces;

public interface IComplaintService
{
    Task<IEnumerable<ComplaintResponseModel>> GetAllComplaintsAsync(int pageList, int pageNumber);
    Task<IEnumerable<ComplaintResponseModel>> GetAllComplaintsByQueueAsync(int id);  
    Task<IEnumerable<ComplaintResponseModel>> GetAllComplaintsByCompanyAsync(int companyId);
    Task<ComplaintResponseModel> GetComplaintByIdAsync(int id);  
    Task<ComplaintResponseModel> AddComplaintAsync(ComplaintRequestModel request);
    Task<ComplaintResponseModel> UpdateComplaintStatusAsync(int id, UpdateComplaintStatusRequest request);
}