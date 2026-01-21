using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Queues.Queries;

public record GetAllQueuesQuery(int PageNumber, int PageSize): IRequest<PagedResponse<QueueResponseModel>>;