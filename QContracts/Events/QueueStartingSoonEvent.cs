using MediatR;

namespace QContracts.Events;

public class QueueStartingSoonEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
}