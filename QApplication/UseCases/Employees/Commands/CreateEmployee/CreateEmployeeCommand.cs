using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.CreateEmployee;

public record CreateEmployeeCommand(int ServiceId, string Firstname, string Lastname, string Position, string PhoneNumber)
    : IRequest<EmployeeResponseModel>;