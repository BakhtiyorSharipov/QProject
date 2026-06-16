using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(int Id, string Firstname, string Lastname, string PhoneNumber)
    : IRequest<CustomerResponseModel>;
