namespace QContracts.NotificationEvents;

public class QueueCanceledByCustomerEvent: BaseEvent
{
    public string? Reason { get; set; }
}