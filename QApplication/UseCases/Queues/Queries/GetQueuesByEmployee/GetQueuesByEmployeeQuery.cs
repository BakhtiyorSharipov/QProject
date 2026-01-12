using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Queues.Queries.GetQueuesByEmployee;

public record GetQueuesByEmployeeQuery(int EmployeeId): IRequest<List<QueueResponseModel>>;