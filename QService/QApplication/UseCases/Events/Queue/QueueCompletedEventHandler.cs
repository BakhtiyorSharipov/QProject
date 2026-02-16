using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QDomain.Events;

namespace QApplication.UseCases.Events.Queue;

public class QueueCompletedEventHandler: INotificationHandler<QueueCompletedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCompletedEventHandler> _logger;

    public QueueCompletedEventHandler(ISmsService smsService, ILogger<QueueCompletedEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Handle(QueueCompletedEvent notification, CancellationToken cancellationToken)
    {
        var message = $"Your queue with Employee {notification.EmployeeId} is now completed.";

        await  _smsService.Send(notification.CustomerId, message, cancellationToken); 
        _logger.LogInformation("QueueCompletedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}