using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Queues.Queries.GetQueuesByCustomer;

public record GetQueuesByCustomerQuery(int CustomerId): IRequest<List<QueueResponseModel>>;