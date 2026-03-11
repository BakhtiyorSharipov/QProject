using MediatR;
using QDomain.Models;

namespace QApplication.UseCases.Auth.Commands.RegisterCustomer;

public record RegisterCustomerCommand(
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber) : IRequest<UserEntity>;