using MediatR;
using User = QDomain.Models.User;

namespace QApplication.UseCases.Auth.Commands.RegisterCustomer;

public record RegisterCustomerCommand(
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber) : IRequest<User>;