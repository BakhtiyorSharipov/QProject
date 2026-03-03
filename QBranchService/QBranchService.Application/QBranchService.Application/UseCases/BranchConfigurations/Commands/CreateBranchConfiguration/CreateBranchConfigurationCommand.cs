using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.CreateBranchConfiguration;

public record CreateBranchConfigurationCommand(
    int BranchId, 
    int MaxTicketsPerDay, 
    TimeOnly OpenTime, 
    TimeOnly CloseTime,
    TimeOnly? BreakStartTime,
    TimeOnly? BreakEndTime)
    : IRequest<BranchConfigurationResponseModel>;