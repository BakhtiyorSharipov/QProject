using MassTransit;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QContracts.SmsEvents;

namespace QInfrastructure.Consumers.Queue;

public class QueueCompletedConsumer: IConsumer<QueueCompletedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCompletedConsumer> _logger;

    public QueueCompletedConsumer(ISmsService smsService, ILogger<QueueCompletedConsumer> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<QueueCompletedEvent> context)
    {
        var notification = context.Message;
        var message = $"Your queue with Employee {notification.EmployeeId} is now completed.";

        await  _smsService.Send(notification.CustomerId, message, context.CancellationToken); 
        _logger.LogInformation("QueueCompletedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}