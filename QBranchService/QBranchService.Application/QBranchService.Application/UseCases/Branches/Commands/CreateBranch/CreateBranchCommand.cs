using MediatR;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Branches.Commands.CreateBranch;

public record CreateBranchCommand(
    string BranchName,
    string City,
    string Address,
    string PhoneNumber,
    string EmailAddress,
    int CompanyId) : IRequest<BranchResponseModel>;