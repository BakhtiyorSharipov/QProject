using MediatR;
using QApplication.Responses;
using QDomain.Models;

namespace QApplication.UseCases.Auth.Commands.CreateEmployee;

public record CreateEmployeeRoleCommand(
    int? ServiceId,
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string Position,
    string PhoneNumber,
    int createdByUserId) : IRequest<User>;