using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery(int PageNumber) : IRequest<PagedResponse<CustomerResponseModel>>;
