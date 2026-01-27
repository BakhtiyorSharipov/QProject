using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QDomain.Events;

namespace QApplication.UseCases.Events.Queue;

public class QueueCanceledByEmployeeEventHandler: INotificationHandler<QueueCanceledByEmployeeEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCanceledByEmployeeEventHandler> _logger;

    public QueueCanceledByEmployeeEventHandler(ISmsService smsService, ILogger<QueueCanceledByEmployeeEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Handle(QueueCanceledByEmployeeEvent notification, CancellationToken cancellationToken)
    {
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by the employee. Reason: {notification.Reason}.. ";

        await _smsService.Send(notification.CustomerId, message, cancellationToken);
        _logger.LogInformation("QueueCanceledByEmployeeEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}