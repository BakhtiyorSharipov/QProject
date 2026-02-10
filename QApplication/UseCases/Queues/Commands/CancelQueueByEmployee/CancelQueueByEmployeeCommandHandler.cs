using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QApplication.Exceptions;
using QApplication.Interfaces.Data;
using QApplication.Responses;
using QDomain.Enums;
using StackExchange.Redis;

namespace QApplication.UseCases.Queues.Commands.CancelQueueByEmployee;

public class CancelQueueByEmployeeCommandHandler: IRequestHandler<CancelQueueByEmployeeCommand, QueueResponseModel>
{
    private readonly ILogger<CancelQueueByEmployeeCommandHandler> _logger;
    private readonly IQueueApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;
  

    public CancelQueueByEmployeeCommandHandler(ILogger<CancelQueueByEmployeeCommandHandler> logger, IQueueApplicationDbContext dbContext, ICacheService cache, IMediator mediator)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<QueueResponseModel> Handle(CancelQueueByEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling queue Id {id} by employee", request.QueueId);

        var dbQueue = await _dbContext.Queues.FirstOrDefaultAsync(s => s.Id == request.QueueId, cancellationToken);
        if (dbQueue == null)
        {
            _logger.LogWarning("Queue with Id {QueueId} not found for employee cancellation", request.QueueId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound);
        }

        dbQueue.Status = QueueStatus.CancelledByEmployee;
        dbQueue.CancelReason = request.CancelReason;
        
        dbQueue.CancelByEmployee();
        
        _logger.LogDebug("Saving employee cancellation changes to repository");
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cache.HashRemoveAsync(CacheKeys.AllQueuesHashKey, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.QueueId(request.QueueId), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.CustomerQueuesHashKey(dbQueue.CustomerId), cancellationToken);

        
        
        var events = dbQueue.DomainEvents.ToList();
        dbQueue.ClearDomainEvents();
        
        foreach (var domainEvent in events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
        
        
        var response = new QueueResponseModel
        {
            Id = dbQueue.Id,
            CustomerId = dbQueue.CustomerId,
            EmployeeId = dbQueue.EmployeeId,
            ServiceId = dbQueue.ServiceId,
            StartTime = dbQueue.StartTime,
            Status = dbQueue.Status
        };

        _logger.LogInformation("Successfully cancelled queue Id {id} by employee", request.QueueId);
        return response;
    }
}