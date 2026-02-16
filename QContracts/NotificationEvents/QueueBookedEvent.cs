

namespace QContracts.NotificationEvents;

public class QueueBookedEvent: BaseEvent
{
    public DateTimeOffset StartTime { get; set; }
    
}