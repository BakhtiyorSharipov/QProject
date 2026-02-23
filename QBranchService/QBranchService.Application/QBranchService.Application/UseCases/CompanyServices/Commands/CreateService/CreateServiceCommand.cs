using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.CreateService;

public record CreateServiceCommand(int CompanyId, string ServiceName, string ServiceDescription): IRequest<CompanyServiceResponseModel>;