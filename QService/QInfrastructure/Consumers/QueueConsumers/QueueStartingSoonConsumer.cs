using MassTransit;
using Microsoft.Extensions.Logging;
using QContracts.NotificationEvents;
using QContracts.QueueEvents;

namespace QInfrastructure.Consumers.QueueConsumers;

public class QueueStartingSoonConsumer:IConsumer<QueueStartingSoonEvent>
{
    private readonly ILogger<QueueStartingSoonConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    public QueueStartingSoonConsumer( ILogger<QueueStartingSoonConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<QueueStartingSoonEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation("Publishing notification event for QueueId {QueueId}", evt.QueueId);
        
        await _publishEndpoint.Publish(new SendNotificationEvent
        {
            UserId = evt.CustomerId,
            Message = $"Reminder: your queue with Employee {evt.EmployeeId} starts in 5 minutes."
        });
        
        _logger.LogInformation("Published notification event for QueueId {QueueId}", evt.QueueId);
        
    }
}