using MediatR;

namespace QBranchService.Application.UseCases.CompanyServices.Commands.DeleteService;

public record DeleteServiceCommand(int Id): IRequest<bool>;