using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Branches.Commands.UpdateBranch;

public record UpdateBranchCommand(
    int Id,
    string BranchName,
    string City,
    string Address,
    string PhoneNumber,
    string EmailAddress) : IRequest<BranchResponseModel>;