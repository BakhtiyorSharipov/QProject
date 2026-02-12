using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.SmsEvents;

namespace QInfrastructure.Consumers.Queue;

public class QueueCanceledByCustomerConsumer: IConsumer<QueueCanceledByCustomerEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCanceledByCustomerConsumer> _logger;

    public QueueCanceledByCustomerConsumer(ISmsService smsService, ILogger<QueueCanceledByCustomerConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QueueCanceledByCustomerEvent> context)
    {
        var notification = context.Message;
        
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by you. Reason: {notification.Reason}.. ";

        await _smsService.Send(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueCanceledByCustomerEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}