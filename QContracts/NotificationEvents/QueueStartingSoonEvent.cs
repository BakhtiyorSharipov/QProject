namespace QContracts.NotificationEvents;

public class QueueStartingSoonEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
}