using MediatR;

namespace QUserService.Application.UseCases.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(int Id): IRequest<bool>;