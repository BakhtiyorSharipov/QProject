using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand( int Id, int ServiceId, string Firstname, string Lastname, string Position, string PhoneNumber)
    : IRequest<EmployeeResponseModel>;