using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.BlockedCustomers.Commands.DeleteBlockedCustomer;

public record DeleteBlockedCustomerCommand(int Id): IRequest<bool>;