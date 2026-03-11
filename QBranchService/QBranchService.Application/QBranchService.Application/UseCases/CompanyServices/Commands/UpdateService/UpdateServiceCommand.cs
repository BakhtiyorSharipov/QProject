using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.UpdateService;

public record UpdateServiceCommand(int Id,int CompanyId, string ServiceName, string ServiceDescription): IRequest<CompanyServiceResponseModel>;