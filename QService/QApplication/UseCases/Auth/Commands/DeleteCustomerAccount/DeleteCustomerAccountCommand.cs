using MediatR;

namespace QApplication.UseCases.Auth.Commands.DeleteCustomerAccount;

public record DeleteCustomerAccountCommand:IRequest<bool>;