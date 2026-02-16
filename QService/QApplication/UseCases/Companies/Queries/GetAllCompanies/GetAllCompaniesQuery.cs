using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Companies.Queries.GetAllCompanies;

public record GetAllCompaniesQuery(int PageNumber): IRequest<PagedResponse<CompanyResponseModel>>;