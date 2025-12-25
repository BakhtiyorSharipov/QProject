using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Enums;
using QDomain.Models;

namespace QApplication.UseCases.Queues.Commands.CancelQueueByCustomer;

public class CancelQueueByCustomerCommandHandler: IRequestHandler<CancelQueueByCustomerCommand, QueueResponseModel>
{
    private readonly ILogger<CancelQueueByCustomerCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;

    public CancelQueueByCustomerCommandHandler(ILogger<CancelQueueByCustomerCommandHandler> logger, IQueueApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<QueueResponseModel> Handle(CancelQueueByCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling queue Id {id} by customer", request.QueueId);
        var dbQueue = await _dbContext.Queues.FirstOrDefaultAsync(s => s.Id == request.QueueId, cancellationToken);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {QueueId} not found for customer cancellation", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound, nameof(QueueEntity));
        }

        _logger.LogDebug("Current queue status: {Status}", dbQueue.Status);
        if (dbQueue.Status != QueueStatus.Pending && dbQueue.Status != QueueStatus.Confirmed)
        {
            _logger.LogWarning("Invalid queue status {Status} for customer cancellation", dbQueue.Status);
            throw new HttpStatusCodeException(HttpStatusCode.BadRequest, nameof(QueueEntity));
        }

        _logger.LogDebug("Time until queue start: {minutes} minutes",
            (dbQueue.StartTime - DateTimeOffset.Now).TotalMinutes);
        if ((dbQueue.StartTime - DateTime.Now).TotalMinutes < 10)
        {
            _logger.LogWarning("Cancellation is than 10 minutes before start time");
            throw new Exception("Cannot cancel less than 10 minutes before start time");
        }

        dbQueue.Status = QueueStatus.CancelledByCustomer;
        dbQueue.CancelReason = request.CancelReason;

        _logger.LogDebug("Saving cancellation changes to repository");
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new QueueResponseModel
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
            Status = dbQueue.Status
        };

        _logger.LogInformation("Successfully cancelled queue Id {id} by customer", request.QueueId);
        return response;
    }
}