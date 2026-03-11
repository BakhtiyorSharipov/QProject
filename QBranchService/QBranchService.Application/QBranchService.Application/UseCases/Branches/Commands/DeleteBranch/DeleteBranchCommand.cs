using MediatR;

namespace QBranchService.Application.UseCases.Branches.Commands.DeleteBranch;

public record DeleteBranchCommand(int Id): IRequest<bool>;