using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.BlockedCustomers.Queries.GetBlockedCustomerById;

public record GetBlockedCustomerByIdQuery(int Id): IRequest<BlockedCustomerResponseModel>;