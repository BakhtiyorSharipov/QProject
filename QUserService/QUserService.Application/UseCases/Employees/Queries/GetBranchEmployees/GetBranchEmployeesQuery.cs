using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Employees.Queries.GetBranchEmployees;

public record GetBranchEmployeesQuery (int CompanyId, int BranchId, int PageNumber): IRequest<PagedResponse<EmployeeInfoResponseModel>>;