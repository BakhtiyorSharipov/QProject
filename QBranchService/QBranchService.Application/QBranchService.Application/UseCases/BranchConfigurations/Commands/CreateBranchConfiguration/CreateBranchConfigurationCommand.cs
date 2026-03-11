using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.BranchConfigurations.Commands.CreateBranchConfiguration;

public record CreateBranchConfigurationCommand(
    int BranchId, 
    int MaxTickets, 
    TimeOnly OpenTime, 
    TimeOnly CloseTime)
    : IRequest<BranchConfigurationResponseModel>;