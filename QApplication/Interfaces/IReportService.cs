using QApplication.Requests.ReportRequest;
using QApplication.Responses.ReportResponse;

namespace QApplication.Interfaces;

public interface IReportService
{
    Task<CompanyReportItemResponseModel> GetCompanyReportAsync(CompanyReportRequest request);
    Task<EmployeeReportResponseModel> GetEmployeeReportAsync(EmployeeReportRequest request);
    Task<QueueReportResponseModel> GetQueueReportAsync(QueueReportRequest request);
    Task<ComplaintReportResponseModel> GetComplaintReportAsync(ComplaintReportRequest request);
    Task<ReviewReportResponseModel> GetReviewReportAsync(ReviewReportRequest request);
    Task<ServiceReportResponseModel> GetServiceReportAsync(ServiceReportRequest request);
    
}