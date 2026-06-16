using MediatR;
using QUserService.Domain.Models;

namespace QUserService.Application.UseCases.Auth.Commands.RegisterCustomer;

public record RegisterCustomerCommand(
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber) : IRequest<UserEntity>;