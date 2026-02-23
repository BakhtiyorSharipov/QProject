using MediatR;
using QApplication.Responses;
using QBranchService.Application.Response;

namespace QBranchService.Application.UseCases.Branches.Queries.GetAllBranches;

public record GetAllBranchesQuery(int PageNumber): IRequest<PagedResponse<BranchResponseModel>>;