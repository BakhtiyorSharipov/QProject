using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.SmsEvents;

namespace QInfrastructure.Consumers.Queue;

public class QueueConfirmedConsumer: IConsumer<QueueConfirmedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueConfirmedConsumer> _logger;

    public QueueConfirmedConsumer(ISmsService smsService, ILogger<QueueConfirmedConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<QueueConfirmedEvent> context)
    {
        var notification = context.Message;
        var message = $"Your queue with Employee {notification.EmployeeId} has been confirmed for {notification.StartTime}.";

        await  _smsService.Send(notification.CustomerId, message, context.CancellationToken); 
        _logger.LogInformation("QueueConfirmedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}