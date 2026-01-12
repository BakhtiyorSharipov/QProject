using MediatR;
using QApplication.Responses.ReportResponse;

namespace QApplication.UseCases.Reports.Queries.GetReviewReport;

public record GetReviewReportQuery(
    int? CompanyId,
    int? EmployeeId,
    int? ServiceId,
    DateTime? From,
    DateTime? To,
    bool IncludeStatistics): IRequest<ReviewReportResponseModel>;