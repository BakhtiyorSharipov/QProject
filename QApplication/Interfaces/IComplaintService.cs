using QApplication.Requests.ComplaintRequest;
using QApplication.Responses;

namespace QApplication.Interfaces;

public interface IComplaintService
{
    IEnumerable<ComplaintResponseModel> GetAllComplaints(int pageList, int pageNumber);
    IEnumerable<ComplaintResponseModel> GetAllComplaintsByQueue(int id);
    ComplaintResponseModel GetComplaintById(int id);
    ComplaintResponseModel AddComplaint(ComplaintRequestModel request);
    ComplaintResponseModel UpdateComplaintStatus(int id, UpdateComplaintStatusRequest request);
}