using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.Events;

namespace QInfrastructure.Consumers.Queue;

public class QueueCanceledByAdminConsumer: IConsumer<QueueCanceledByAdminEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCanceledByAdminConsumer> _logger;

    public QueueCanceledByAdminConsumer(ISmsService smsService, ILogger<QueueCanceledByAdminConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<QueueCanceledByAdminEvent> context)
    {
        var notification = context.Message;
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by admin. Reason: {notification.Reason}.. ";

        await _smsService.Send(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueCanceledByAdminEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}