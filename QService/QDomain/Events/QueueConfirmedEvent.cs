using MediatR;

namespace QDomain.Events;

public class QueueConfirmedEvent: BaseEvent, INotification
{
    public DateTimeOffset StartTime { get; set; }

}