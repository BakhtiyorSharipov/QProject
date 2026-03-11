using MediatR;
using QApplication.Responses;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.CompanyServices.Queries.GetAllServices;

public record GetAllServicesQuery(int PageNumber): IRequest<PagedResponse<CompanyServiceResponseModel>>;