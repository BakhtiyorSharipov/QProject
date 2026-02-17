using System.Collections.Frozen;
using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QContracts.NotificationEvents;
using QContracts.QueueEvents;
using QContracts.QueueEvents.Enums;
using QInfrastructure.Extensions;

namespace QInfrastructure.Consumers.QueueConsumers;

public class QueueUpdatedEventConsumer : IConsumer<QueueUpdatedEvent>
{
    private readonly ILogger<QueueUpdatedEventConsumer> _logger;
    private readonly ICacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;

  

    private static readonly FrozenDictionary<UpdatedQueueStatus, string> StatusMessage =
        new Dictionary<UpdatedQueueStatus, string>()
        {
            [UpdatedQueueStatus.CanceledByCustomer] =
                "Your queue with Employee {0} was canceled by you. Reason: {1}.. ",
            [UpdatedQueueStatus.CanceledByEmployee] =
                "Your queue with Employee {0} was canceled by employee. Reason: {1}.. ",
            [UpdatedQueueStatus.CanceledByAdmin] = "Your queue with Employee {0} was canceled by admin. Reason: {1}.. ",
            [UpdatedQueueStatus.Completed] = "Your queue with Employee {0} is now completed. ",
            [UpdatedQueueStatus.Confirmed] = "Your queue with Employee {0} has been confirmed for {1}. "
        }.ToFrozenDictionary();


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

        var cacheReset=_cacheService.ResetCacheAsync(evt.QueueId, evt.CustomerId, evt.EmployeeId, context.CancellationToken);


        _logger.LogInformation("Publishing notification event for QueueId {QueueId}", evt.QueueId);

        Task? notificationTask = null;
        if (
            StatusMessage.TryGetValue(evt.Status, out var template))
        {
            var message = string.Format(template, evt.EmployeeId, evt.CancelReason ?? evt.StartTime.ToString());
            notificationTask= _publishEndpoint.Publish(new SendNotificationEvent
            {
                UserId = evt.CustomerId,
                Message = message
            });
        }

        if (notificationTask!=null)
        {
            await Task.WhenAll(cacheReset, notificationTask);
        }
        else
        {
            await cacheReset;
        }
        
        _logger.LogInformation("Cache reset processed for QueueId {QueueId}", evt.QueueId);
        _logger.LogInformation("Published notification event for QueueId {QueueId}", evt.QueueId);
    }
}