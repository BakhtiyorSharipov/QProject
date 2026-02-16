using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QDomain.Events;

namespace QApplication.UseCases.Events.Queue;

public class QueueCanceledByCustomerEventHandler: INotificationHandler<QueueCanceledByCustomerEvent>
{

    private readonly ISmsService _smsService;
    private readonly ILogger<QueueCanceledByCustomerEventHandler> _logger;

    public QueueCanceledByCustomerEventHandler(ISmsService smsService, ILogger<QueueCanceledByCustomerEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Handle(QueueCanceledByCustomerEvent notification, CancellationToken cancellationToken)
    {
        var message =
            $"Your queue with Employee {notification.EmployeeId} was canceled by you. Reason: {notification.Reason}.. ";

        await _smsService.Send(notification.CustomerId, message, cancellationToken);
        _logger.LogInformation("QueueCanceledByCustomerEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}