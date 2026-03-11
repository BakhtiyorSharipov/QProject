using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Queries.GetBranchConfigurationById;

public record GetBranchConfigurationByIdQuery(int Id): IRequest<BranchConfigurationResponseModel>;