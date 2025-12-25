using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.BlockedCustomers.Queries.GetAllBlockedCustomers;

public record GetAllBlockedCustomersQuery(int PageNumber, int PageSize): IRequest<PagedResponse<BlockedCustomerResponseModel>>;