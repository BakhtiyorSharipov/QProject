using MediatR;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Auth.Commands.CreateEmployee;

public record CreateEmployeeRoleCommand(
    int BranchId,
    int? ServiceId,
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string Position,
    string PhoneNumber,
    int createdByUserId) : IRequest<UserEntity>;