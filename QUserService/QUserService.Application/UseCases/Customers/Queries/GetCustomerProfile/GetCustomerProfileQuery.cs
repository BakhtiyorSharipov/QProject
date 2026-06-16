using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.Customers.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery: IRequest<CustomerProfileResponse>;