using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand( int Id, int ServiceId, string Firstname, string Lastname, string Position, string PhoneNumber)
    : IRequest<EmployeeResponseModel>;