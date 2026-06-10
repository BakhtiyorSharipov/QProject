using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Queries.GetBranchEmployees;

public record GetBranchEmployeesQuery (int CompanyId, int BranchId, int PageNumber): IRequest<PagedResponse<EmployeeInfoResponseModel>>;