using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Queries.GetAllEmployees;

public record GetAllEmployeesQuery(int PageNumber): IRequest<PagedResponse<EmployeeResponseModel>>;