using QApplication.Requests.ReportRequest;
using QApplication.Responses.ReportResponse;

namespace QApplication.Interfaces;

public interface IReportService
{
    CompanyReportItemResponseModel GetCompanyReport(CompanyReportRequest request);
    EmployeeReportResponseModel GetEmployeeReport(EmployeeReportRequest request);
    QueueReportResponseModel GetQueueReport(QueueReportRequest request);
    ComplaintReportResponseModel GetComplaintReport(ComplaintReportRequest request);
    ReviewReportResponseModel GetReviewReport( ReviewReportRequest request);
    ServiceReportResponseModel GetServiceReport(ServiceReportRequest request);
    
    
}