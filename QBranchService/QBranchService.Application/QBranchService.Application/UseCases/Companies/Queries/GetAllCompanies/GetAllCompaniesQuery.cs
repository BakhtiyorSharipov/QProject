using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Companies.Queries.GetAllCompanies;

public record GetAllCompaniesQuery(int PageNumber): IRequest<PagedResponse<CompanyResponseModel>>;