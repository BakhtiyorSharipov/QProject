using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(int Id): IRequest<CompanyResponseModel>;