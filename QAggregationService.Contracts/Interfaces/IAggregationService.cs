using MagicOnion;
using QAggregationService.Contracts.Requests;
using QAggregationService.Contracts.Responses;

namespace QAggregationService.Contracts.Interfaces;

public interface IAggregationService
{
    Task<CompanyReportResponse> GetReportAsync(ReportRequest request);
    Task<DashboardResponse> GetCompanyDashboard( int companyId);

    Task<EmployeeReportResponse> GetEmployeeReport(EmployeeReportRequest request);
    Task<CustomerReportResponse> GetCustomerReport(CustomerReportRequest request);
}