using QApplication.Responses.ReportResponse;

namespace QApplication.Interfaces;

public interface IReportService
{
    EmployeeReportResponseModel GetEmployeeReport(int employeeId);
    QueueReportResponseModel GetQueueReport();
    ComplaintReportResponseModel GetComplaintReport();
    ReviewReportResponseModel GetReviewReport();
}