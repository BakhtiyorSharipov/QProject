using MediatR;

namespace QUserService.Application.UseCases.Auth.Commands.DeleteCustomerAccount;

public record DeleteCustomerAccountCommand:IRequest<bool>;