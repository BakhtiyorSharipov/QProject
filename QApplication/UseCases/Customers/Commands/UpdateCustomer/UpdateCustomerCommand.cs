using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(int Id, string Firstname, string Lastname, string PhoneNumber)
    : IRequest<CustomerResponseModel>;
