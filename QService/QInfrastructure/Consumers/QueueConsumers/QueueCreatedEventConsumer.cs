using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Caching;
using QContracts.NotificationEvents;
using QContracts.QueueEvents;
using QInfrastructure.Extensions;

namespace QInfrastructure.Consumers.QueueConsumers;

public class QueueCreatedEventConsumer : IConsumer<QueueCreatedEvent>
{
    private readonly ILogger<QueueCreatedEventConsumer> _logger;
    private readonly ICacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;

    public QueueCreatedEventConsumer(ILogger<QueueCreatedEventConsumer> logger, ICacheService cacheService,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<QueueCreatedEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Processing cache reset for QueueId {QueueId}", evt.QueueId);

        var cacheRest =
            _cacheService.ResetCacheAsync(evt.QueueId, evt.CustomerId, evt.EmployeeId, context.CancellationToken);


        _logger.LogInformation("Publishing notification event for QueueId {QueueId}", evt.QueueId);

        var notification = _publishEndpoint.Publish(new SendNotificationEvent()
        {
            UserId = evt.CustomerId,
            Message = $"You have successfully booked a queue with Employee {evt.EmployeeId} at {evt.StartTime}. "
        });

        await Task.WhenAll(cacheRest, notification);

        _logger.LogInformation("Cache reset processed for QueueId {QueueId}", evt.QueueId);

        _logger.LogInformation("Published notification event for QueueId {QueueId}", evt.QueueId);
    }
}