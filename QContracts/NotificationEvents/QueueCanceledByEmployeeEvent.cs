namespace QContracts.NotificationEvents;

public class QueueCanceledByEmployeeEvent: BaseEvent
{
    public string? Reason { get; set; }
}