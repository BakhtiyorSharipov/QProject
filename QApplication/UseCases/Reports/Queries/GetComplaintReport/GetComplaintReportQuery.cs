using MediatR;
using QApplication.Responses.ReportResponse;
using QDomain.Enums;

namespace QApplication.UseCases.Reports.Queries.GetComplaintReport;

public record GetComplaintReportQuery(
    int? CompanyId,
    int? EmployeeId,
    int? ServiceId,
    DateTime? From,
    DateTime? To,
    ComplaintStatus? Status,
    bool IncludeStatistics): IRequest<ComplaintReportResponseModel>;