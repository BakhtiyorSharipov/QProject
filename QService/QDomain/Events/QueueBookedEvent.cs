using MediatR;

namespace QDomain.Events;

public class QueueBookedEvent: BaseEvent, INotification
{
    public DateTimeOffset StartTime { get; set; }
    
}