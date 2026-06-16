using MediatR;

namespace QUserService.Application.UseCases.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(int Id): IRequest<bool>;