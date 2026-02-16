using MassTransit;
using Microsoft.Extensions.Logging;
using QContracts.NotificationEvents;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Infrastructure.Consumers;

public class QueueBookedConsumer: IConsumer<QueueBookedEvent>
{
    private readonly INotificationService _smsService;
    private readonly ILogger<QueueBookedConsumer> _logger;

    public QueueBookedConsumer(INotificationService smsService, ILogger<QueueBookedConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QueueBookedEvent> context)
    {
        var notification = context.Message;
        var message =
            $"You have successfully booked a queue with Employee {notification.EmployeeId} at {notification.StartTime}. ";

        await _smsService.SendAsync(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueBookedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}