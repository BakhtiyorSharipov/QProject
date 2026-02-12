using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.SmsEvents;

namespace QInfrastructure.Consumers.Queue;

public class QueueStartingSoonConsumer:IConsumer<QueueStartingSoonEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueStartingSoonConsumer> _logger;

    public QueueStartingSoonConsumer(ISmsService smsService, ILogger<QueueStartingSoonConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QueueStartingSoonEvent> context)
    {
        var notification = context.Message;
        var message = $"Reminder: your queue with Employee {notification.EmployeeId} starts in 5 minutes.";

        await _smsService.Send(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueStartingSoonEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}