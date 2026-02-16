using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(int Id): IRequest<EmployeeResponseModel>;