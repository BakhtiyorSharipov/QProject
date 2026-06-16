using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(int Id): IRequest<EmployeeResponseModel>;