

namespace QContracts.SmsEvents;

public class QueueBookedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
    
}