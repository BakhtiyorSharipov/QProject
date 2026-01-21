using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Queries.GetAllEmployees;

public record GetAllEmployeesQuery(int pageNumber, int pageSize): IRequest<PagedResponse<EmployeeResponseModel>>;