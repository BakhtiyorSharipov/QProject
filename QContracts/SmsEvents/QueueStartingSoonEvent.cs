namespace QContracts.SmsEvents;

public class QueueStartingSoonEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
}