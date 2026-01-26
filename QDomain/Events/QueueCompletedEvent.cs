using MediatR;

namespace QDomain.Events;

public class QueueCompletedEvent: BaseEvent, INotification
{
    public DateTimeOffset StartTime { get; set; }
}