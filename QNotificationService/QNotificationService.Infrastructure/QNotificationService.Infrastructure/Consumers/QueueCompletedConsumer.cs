using MassTransit;
using Microsoft.Extensions.Logging;
using QContracts.NotificationEvents;
using QNotificationService.Application.Interfaces;


namespace QNotificationService.Infrastructure.Consumers;

public class QueueCompletedConsumer: IConsumer<QueueCompletedEvent>
{
    private readonly INotificationService _smsService;
    private readonly ILogger<QueueCompletedConsumer> _logger;

    public QueueCompletedConsumer(INotificationService smsService, ILogger<QueueCompletedConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<QueueCompletedEvent> context)
    {
        var notification = context.Message;
        var message = $"Your queue with Employee {notification.EmployeeId} is now completed.";

        await  _smsService.SendAsync(notification.CustomerId, message, context.CancellationToken); 
        _logger.LogInformation("QueueCompletedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}