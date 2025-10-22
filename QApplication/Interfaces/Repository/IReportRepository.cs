using QApplication.Responses.ReportResponse;
using QDomain.Models;

namespace QApplication.Interfaces.Repository;

public interface IReportRepository
{
    IQueryable<QueueEntity> GetEmployeeReport(int employeeId);
    IQueryable<QueueEntity> GetQueueReport();
    IQueryable<ComplaintEntity> GetComplaintReport();
    IQueryable<ReviewEntity> GetReviewReport();
}