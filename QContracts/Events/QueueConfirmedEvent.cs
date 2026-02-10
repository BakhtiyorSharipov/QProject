using MediatR;

namespace QContracts.Events;

public class QueueConfirmedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }

}