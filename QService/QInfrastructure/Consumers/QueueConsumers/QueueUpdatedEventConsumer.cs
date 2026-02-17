using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QContracts.NotificationEvents;
using QContracts.NotificationEvents.Enums;
using QContracts.QueueEvents;
using QContracts.QueueEvents.Enums;
using QInfrastructure.Extensions;

namespace QInfrastructure.Consumers.QueueConsumers;

public class QueueUpdatedEventConsumer: IConsumer<QueueUpdatedEvent>
{
    private readonly ILogger<QueueUpdatedEventConsumer> _logger;
    private readonly ICacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;

    public QueueUpdatedEventConsumer(ILogger<QueueUpdatedEventConsumer> logger, ICacheService cacheService,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<QueueUpdatedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Processing cache reset for QueueId {QueueId}", evt.QueueId);

        await _cacheService.ResetCacheAsync(evt.QueueId, evt.CustomerId, evt.EmployeeId, context.CancellationToken);

        _logger.LogInformation("Cache reset processed for QueueId {QueueId}", evt.QueueId);

        _logger.LogInformation("Publishing notification event for QueueId {QueueId}", evt.QueueId);
        if (evt.Status == UpdatedQueueStatus.CanceledByEmployee)
        {
            await _publishEndpoint.Publish(new SendNotificationEvent
            {
                UserId = evt.CustomerId,
                Title = NotificationTitle.CanceledByEmployee,
                Message = $"Your queue with Employee {evt.EmployeeId} was canceled by employee. Reason: {evt.CancelReason}.. "
            });
        }
        else if (evt.Status == UpdatedQueueStatus.CanceledByCustomer)
        {
            await _publishEndpoint.Publish(new SendNotificationEvent
            {
                UserId = evt.CustomerId,
                Title = NotificationTitle.CanceledByCustomer,
                Message = $"Your queue with Employee {evt.EmployeeId} was canceled by you. Reason: {evt.CancelReason}.. "
            });
        }
        else if (evt.Status== UpdatedQueueStatus.CanceledByAdmin)
        {
            await _publishEndpoint.Publish(new SendNotificationEvent
            {
                UserId = evt.CustomerId,
                Title = NotificationTitle.CanceledByAdmin,
                Message = $"Your queue with Employee {evt.EmployeeId} was canceled by admin. Reason: {evt.CancelReason}.. "
            });
        }
        else if (evt.Status== UpdatedQueueStatus.Confirmed)
        {
            await _publishEndpoint.Publish(new SendNotificationEvent
            {
                UserId = evt.CustomerId,
                Title = NotificationTitle.Confirmed,
                Message = $"Your queue with Employee {evt.EmployeeId} has been confirmed for {evt.StartTime}. "
            });
        }
        else if (evt.Status == UpdatedQueueStatus.Completed)
        {
            await _publishEndpoint.Publish(new SendNotificationEvent
            {
                UserId = evt.CustomerId,
                Title = NotificationTitle.Completed,
                Message = $"Your queue with Employee {evt.EmployeeId} is now completed. "
            });
        }
        
        _logger.LogInformation("Published notification event for QueueId {QueueId}", evt.QueueId);
    }
}