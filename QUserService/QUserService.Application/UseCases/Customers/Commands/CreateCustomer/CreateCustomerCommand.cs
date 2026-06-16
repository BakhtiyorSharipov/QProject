using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(string Firstname, string Lastname, string PhoneNumber) : IRequest<CustomerResponseModel>;
