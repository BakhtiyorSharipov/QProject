namespace QContracts.NotificationEvents;

public class QueueCompletedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
}