using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.UpdateBranchConfiguration;

public record UpdateBranchConfigurationCommand(
    int Id,
    int MaxTickest,
    TimeOnly OpenTime,
    TimeOnly CloseTime): IRequest<BranchConfigurationResponseModel>;