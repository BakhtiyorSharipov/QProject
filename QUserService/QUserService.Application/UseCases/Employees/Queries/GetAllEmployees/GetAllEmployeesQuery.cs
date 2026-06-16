using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Employees.Queries.GetAllEmployees;

public record GetAllEmployeesQuery(int PageNumber): IRequest<PagedResponse<EmployeeResponseModel>>;