using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QDomain.Events;

namespace QApplication.UseCases.Events.Queue;

public class QueueConfirmedEventHandler: INotificationHandler<QueueConfirmedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueConfirmedEventHandler> _logger;

    public QueueConfirmedEventHandler(ISmsService smsService, ILogger<QueueConfirmedEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Handle(QueueConfirmedEvent notification, CancellationToken cancellationToken)
    {
        var message = $"Your queue with Employee {notification.EmployeeId} has been confirmed for {notification.StartTime}.";

        await  _smsService.Send(notification.CustomerId, message, cancellationToken); 
        _logger.LogInformation("QueueConfirmedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
        
    }
}