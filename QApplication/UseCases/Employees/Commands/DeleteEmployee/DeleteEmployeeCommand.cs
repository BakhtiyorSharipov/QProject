using MediatR;

namespace QApplication.UseCases.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(int Id): IRequest<bool>;