using MediatR;

namespace QApplication.UseCases.Employees.DeleteEmployee;

public record DeleteEmployeeCommand(int Id): IRequest<bool>;