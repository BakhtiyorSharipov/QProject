using MassTransit;
using Microsoft.Extensions.Logging;
using QContracts.NotificationEvents;
using QNotificationService.Application.Interfaces;

namespace QNotificationService.Infrastructure.Consumers;

public class QueueCanceledByAdminConsumer: IConsumer<QueueCanceledByAdminEvent>
{
    private readonly INotificationService _smsService;
    private readonly ILogger<QueueCanceledByAdminConsumer> _logger;

    public QueueCanceledByAdminConsumer(INotificationService smsService, ILogger<QueueCanceledByAdminConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<QueueCanceledByAdminEvent> context)
    {
        var notification = context.Message;
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by admin. Reason: {notification.Reason}.. ";

        await _smsService.SendAsync(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueCanceledByAdminEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}