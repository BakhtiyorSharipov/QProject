using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Companies.Queries.GetCompanyInfoById;

public record GetCompanyInfoByIdQuery(int Id) : IRequest<CompanyByIdResponseModel>; 