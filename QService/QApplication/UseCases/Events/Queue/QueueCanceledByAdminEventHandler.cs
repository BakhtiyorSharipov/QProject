using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QDomain.Events;

namespace QApplication.UseCases.Events.Queue;

public class QueueCanceledByAdminEventHandler: INotificationHandler<QueueCanceledByAdminEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCanceledByAdminEventHandler> _logger;

    public QueueCanceledByAdminEventHandler(ISmsService smsService, ILogger<QueueCanceledByAdminEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Handle(QueueCanceledByAdminEvent notification, CancellationToken cancellationToken)
    {
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by admin. Reason: {notification.Reason}.. ";

        await _smsService.Send(notification.CustomerId, message, cancellationToken);
        _logger.LogInformation("QueueCanceledByAdminEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}