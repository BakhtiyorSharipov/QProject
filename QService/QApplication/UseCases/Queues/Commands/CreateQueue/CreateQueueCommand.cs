using MediatR;
using QApplication.Responses;

namespace QApplication.UseCases.Queues.Commands.CreateQueue;

public record CreateQueueCommand(int CompanyId, int BranchId, int EmployeeId, int CustomerId, int ServiceId, DateTimeOffset StartTime) : IRequest<AddQueueResponseModel>;