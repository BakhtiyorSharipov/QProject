namespace QContracts.NotificationEvents;

public class QueueConfirmedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }

}