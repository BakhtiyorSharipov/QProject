using MediatR;

namespace QDomain.Events;

public class QueueStartingSoonEvent: BaseEvent, INotification
{
    public DateTimeOffset StartTime { get; set; }
}