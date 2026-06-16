using MediatR;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Auth.Commands.CreateCompanyAdmin;

public record CreateCompanyAdminCommand(
    int CompanyId,
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string Position,
    string PhoneNumber,
    int createdByUserId): IRequest<UserEntity>;