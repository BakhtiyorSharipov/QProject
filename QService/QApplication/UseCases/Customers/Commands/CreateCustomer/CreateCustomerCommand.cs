using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(string Firstname, string Lastname, string PhoneNumber) : IRequest<CustomerResponseModel>;
