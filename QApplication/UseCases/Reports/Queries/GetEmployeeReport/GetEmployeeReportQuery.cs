using MediatR;
using QApplication.Responses.ReportResponse;

namespace QApplication.UseCases.Reports.Queries.GetEmployeeReport;

public record GetEmployeeReportQuery(int? CompanyId, int? EmployeeId, int? ServiceId, DateTime? From, DateTime? To)
    : IRequest<EmployeeReportResponseModel>;