using MediatR;
using Microsoft.Extensions.Logging;
using QApplication.Interfaces;
using QDomain.Events;


namespace QApplication.UseCases.Events.Queue;

public  class QueueBookedEventHandler: INotificationHandler<QueueBookedEvent>
{
    private readonly ISmsService _smsService;
    private readonly ILogger<QueueBookedEventHandler> _logger;

    public QueueBookedEventHandler(ISmsService smsService, ILogger<QueueBookedEventHandler> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }


    public  async Task Handle(QueueBookedEvent notification, CancellationToken cancellationToken)
    {
        var message =
            $"You have successfully booked a queue with Employee {notification.EmployeeId} at {notification.StartTime}. ";

        await _smsService.Send(notification.CustomerId, message, cancellationToken);
        _logger.LogInformation("QueueBookedEvent SMS sent to customer {CustomerId}", notification.CustomerId);
    }
}