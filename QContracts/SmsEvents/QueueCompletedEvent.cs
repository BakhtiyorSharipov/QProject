namespace QContracts.SmsEvents;

public class QueueCompletedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
}