using MediatR;

namespace QUserService.Application.UseCases.BlockedCustomers.Commands.DeleteBlockedCustomer;

public record DeleteBlockedCustomerCommand(int Id): IRequest<bool>;