using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.SmsEvents;

namespace QInfrastructure.Consumers.Queue;

public class QueueCanceledByEmployeeConsumer : IConsumer<QueueCanceledByEmployeeEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCanceledByEmployeeConsumer> _logger;

    public QueueCanceledByEmployeeConsumer(ISmsService smsService, ILogger<QueueCanceledByEmployeeConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<QueueCanceledByEmployeeEvent> context)
    {
        var notification = context.Message;

        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by employee. Reason: {notification.Reason}.. ";

        await _smsService.Send(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueCanceledByCustomerEvent SMS sent to customer {CustomerId}",
            notification.CustomerId);
    }
}