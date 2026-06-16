using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Employees.Queries.GetServiceEmployees;

public record GetServiceEmployeesQuery (int CompanyId, int ServiceId, int PageNumber): IRequest<PagedResponse<EmployeeInfoResponseModel>>;