using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Branches.Queries.GetBranchById;

public record GetBranchByIdQuery(int Id): IRequest<BranchResponseModel>;