using MediatR;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.DeleteBranchConfiguration;

public record DeleteBranchConfigurationCommand(int Id): IRequest<bool>;