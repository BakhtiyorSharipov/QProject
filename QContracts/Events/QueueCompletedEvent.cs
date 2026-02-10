using MediatR;

namespace QContracts.Events;

public class QueueCompletedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
}