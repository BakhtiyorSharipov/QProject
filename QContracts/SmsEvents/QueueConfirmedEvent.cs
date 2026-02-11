namespace QContracts.SmsEvents;

public class QueueConfirmedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }

}