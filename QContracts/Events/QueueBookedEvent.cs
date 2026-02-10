using MediatR;

namespace QContracts.Events;

public class QueueBookedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
    
}