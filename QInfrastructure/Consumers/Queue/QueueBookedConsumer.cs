using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.SmsEvents;

namespace QInfrastructure.Consumers.Queue;

public class QueueBookedConsumer: IConsumer<QueueBookedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueBookedConsumer> _logger;

    public QueueBookedConsumer(ISmsService smsService, ILogger<QueueBookedConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<QueueBookedEvent> context)
    {
        var notification = context.Message;
        var message =
            $"You have successfully booked a queue with Employee {notification.EmployeeId} at {notification.StartTime}. ";

        await _smsService.Send(notification.CustomerId, message, context.CancellationToken);
        _logger.LogInformation("QueueBookedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}