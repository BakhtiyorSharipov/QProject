using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Queries.GetAllBranchConfigurations;

public record GetAllBranchConfigurationsQuery(int PageNumber): IRequest<PagedResponse<BranchConfigurationResponseModel>>;