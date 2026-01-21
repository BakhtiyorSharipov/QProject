using MediatR;
using QApplication.Responses.ReportResponse;

namespace QApplication.UseCases.Reports.Queries;

public record GetCompanyReportQuery(int CompanyId, DateTime? From, DateTime? To): IRequest<CompanyReportItemResponseModel>;