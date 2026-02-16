using MassTransit;
using Microsoft.Extensions.Logging;
using QContracts.NotificationEvents;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Infrastructure.Consumers;

public class QueueStartingSoonConsumer:IConsumer<QueueStartingSoonEvent>
{
    private readonly INotificationService _smsService;
    private readonly ILogger<QueueStartingSoonConsumer> _logger;

    public QueueStartingSoonConsumer(INotificationService smsService, ILogger<QueueStartingSoonConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QueueStartingSoonEvent> context)
    {
        var notification = context.Message;
        var message = $"Reminder: your queue with Employee {notification.EmployeeId} starts in 5 minutes.";

        await _smsService.SendAsync(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueStartingSoonEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}