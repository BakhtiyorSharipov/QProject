using MediatR;
using QDomain.Models;

namespace QApplication.UseCases.Auth.Commands.CreateCompanyAdmin;

public record CreateCompanyAdminCommand(
    int ServiceId,
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string Position,
    string PhoneNumber,
    int createdByUserId): IRequest<User>;