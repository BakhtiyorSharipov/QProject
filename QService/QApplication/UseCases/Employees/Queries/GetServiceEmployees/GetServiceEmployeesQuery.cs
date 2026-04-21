using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Queries.GetServiceEmployees;

public record GetServiceEmployeesQuery (int CompanyId, int ServiceId, int PageNumber): IRequest<PagedResponse<EmployeeInfoResponseModel>>;