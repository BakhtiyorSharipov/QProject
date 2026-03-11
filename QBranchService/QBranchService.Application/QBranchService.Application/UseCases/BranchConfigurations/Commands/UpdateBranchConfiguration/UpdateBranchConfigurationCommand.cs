using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.UpdateBranchConfiguration;

public record UpdateBranchConfigurationCommand(
    int Id,
    int MaxTicketsPerDay,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    TimeOnly? BreakStartTime,
    TimeOnly? BreakEndTime): IRequest<BranchConfigurationResponseModel>;