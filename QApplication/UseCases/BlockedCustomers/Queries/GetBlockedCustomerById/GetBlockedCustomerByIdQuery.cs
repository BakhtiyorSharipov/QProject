using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.BlockedCustomers.Queries.GetBlockedCustomerById;

public record GetBlockedCustomerByIdQuery(int Id): IRequest<BlockedCustomerResponseModel>;