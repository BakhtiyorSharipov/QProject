using MediatR;
using QApplication.Responses.ReportResponse;

namespace QApplication.UseCases.Reports.Queries.GetServiceReport;

public record GetServiceReportQuery(int? CompanyId, int? ServiceId): IRequest<ServiceReportResponseModel>;