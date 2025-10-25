using QApplication.Requests.QueueRequest;
using QApplication.Requests.ReportRequest;
using QApplication.Responses.ReportResponse;
using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IReportRepository
{
    IQueryable<EmployeeEntity> GetEmployeeReport(EmployeeReportRequest request);
    IQueryable<QueueEntity> GetQueueReport(QueueReportRequest request);
    IQueryable<ComplaintEntity> GetComplaintReport(ComplaintReportRequest request);
    IQueryable<ReviewEntity> GetReviewReport(ReviewReportRequest request);

    IQueryable<ServiceEntity> GetServiceReport(ServiceReportRequest request);
    
}