using MassTransit;
using Microsoft.Extensions.Logging;
using QContracts.NotificationEvents;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Infrastructure.Consumers;

public class QueueCanceledByCustomerConsumer: IConsumer<QueueCanceledByCustomerEvent>
{
    private readonly INotificationService _smsService;
    private readonly ILogger<QueueCanceledByCustomerConsumer> _logger;

    public QueueCanceledByCustomerConsumer(INotificationService smsService, ILogger<QueueCanceledByCustomerConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QueueCanceledByCustomerEvent> context)
    {
        var notification = context.Message;
        
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by you. Reason: {notification.Reason}.. ";

        await _smsService.SendAsync(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueCanceledByCustomerEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}