using MediatR;
using QApplication.Responses.ReportResponse;
using QDomain.Enums;

namespace QApplication.UseCases.Reports.Queries.GetQueueReport;

public record GetQueueReportQuery(
    int? CompanyId,
    int? EmployeeId,
    int? ServiceId,
    DateTime? From,
    DateTime? To,
    QueueStatus? Status,
    bool IncludeStatistics) : IRequest<QueueReportResponseModel>;