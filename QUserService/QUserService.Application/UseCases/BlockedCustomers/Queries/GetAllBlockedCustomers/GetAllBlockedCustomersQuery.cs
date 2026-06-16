using MediatR;
using QUserService.Application.Responses;

namespace QUserService.Application.UseCases.BlockedCustomers.Queries.GetAllBlockedCustomers;

public record GetAllBlockedCustomersQuery(int PageNumber): IRequest<PagedResponse<BlockedCustomerResponseModel>>;