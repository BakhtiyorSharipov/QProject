using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(int Id): IRequest<bool>;