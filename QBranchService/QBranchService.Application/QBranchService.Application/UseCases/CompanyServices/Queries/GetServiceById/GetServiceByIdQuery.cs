using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.CompanyServices.Queries.GetServiceById;

public record GetServiceByIdQuery(int Id): IRequest<ServiceResponseModel>;