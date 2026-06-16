using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(int Id): IRequest<CustomerResponseModel>;